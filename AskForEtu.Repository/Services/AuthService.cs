using AskForEtu.Core.Behaviour;
using AskForEtu.Core.Dto.Mail;
using AskForEtu.Core.Dto.Request;
using AskForEtu.Core.Dto.Response;
using AskForEtu.Core.Entity;
using AskForEtu.Core.Hash;
using AskForEtu.Core.JwtGenerator;
using AskForEtu.Core.ResultStructure;
using AskForEtu.Core.ResultStructure.Dto;
using AskForEtu.Core.Services;
using AskForEtu.Core.Services.Queue;
using AskForEtu.Core.Services.Repo;
using AskForEtu.Repository.UnitofWork;
using AutoMapper;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Security.Cryptography;

namespace AskForEtu.Repository.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<AuthService> _logger;
        private readonly IFacultRepository _facultRepository;
        private readonly IMajorRepository _majorRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly IJwtProvider _provider;
        private readonly ITaskQueue<EmailSendTemplate> _queue;
        private readonly LinkGenerator _linkGenerator;
        private readonly IPasswordResetRepository _pwdResetRepository;

        public AuthService(
            IUserRepository userRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            ILogger<AuthService> logger,
            IFacultRepository facultRepository,
            IMajorRepository majorRepository,
            ITokenRepository tokenRepository,
            IJwtProvider provider,
            ITaskQueue<EmailSendTemplate> queue,
            LinkGenerator linkGenerator,
            IPasswordResetRepository pwdResetRepository,
            IRoleRepository roleRepository)
        {
            _userRepository=userRepository;
            _mapper=mapper;
            _unitOfWork=unitOfWork;
            _passwordHasher=passwordHasher;
            _logger=logger;
            _facultRepository=facultRepository;
            _majorRepository=majorRepository;
            _tokenRepository=tokenRepository;
            _provider=provider;
            _queue=queue;
            _linkGenerator=linkGenerator;
            _pwdResetRepository=pwdResetRepository;
            _roleRepository=roleRepository;
        }

        public async Task<Response<TokenDto>> LoginAsync(LoginDto loginDto)
        {
            int statusCode = StatusCodes.Status200OK;
            try
            {
                var user = await _userRepository.GetByUserOrEmailAsync(loginDto.userNameOrEmail);

                if (user.IsDeleted)
                {
                    statusCode = StatusCodes.Status401Unauthorized;
                    throw new InvalidDataException("Bu kullanıcı banlanmıştır.");
                }

                if (user is not User)
                {
                    statusCode = StatusCodes.Status404NotFound;
                    throw new InvalidDataException($"Kullanıcı bulunamadı.");
                }

                if (!_passwordHasher.Verify(user.PasswordHash, loginDto.password))
                {
                    statusCode = StatusCodes.Status401Unauthorized;
                    throw new InvalidDataException($"Kullanıcı adı ya da şifre hatalı. {user.Email}");
                }

                if (!user.VerifyEmail)
                {
                    statusCode = StatusCodes.Status401Unauthorized;
                    throw new InvalidDataException("Email doğrulaması yapılmadı. Lütfen email adresinizi doğrulayın.");
                }

                Token userToken = user.Token;
                if (user.Token is not Token)
                {
                    userToken = new();
                    userToken.UserId = user.Id;
                }

                // Kullacini hesabi pasif almissa tekrar giriste aktif olarak guncelleniyor 
                if (!user.IsActive) user.IsActive = true;


                var userTokenResponse = CreateToken(user, userToken, true);

                userToken.AccessToken = userTokenResponse.token;

                _userRepository.Update(user);
                _tokenRepository.Update(userToken);
                await _unitOfWork.SaveAsync();

                return Response<TokenDto>
                    .Success(userTokenResponse, StatusCodes.Status200OK);
            }
            catch (InvalidDataException err)
            {
                _logger.LogWarning(err.Message);
                return Response<TokenDto>
                    .Fail(err.Message.Split(".")[0], statusCode);
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
                return Response<TokenDto>
                    .Fail("Bir seyler ters gitti.", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<Response<NoContent>> RegisterAsync(RegisterDto registerDto, HttpContext context)
        {
            try
            {
                if (!registerDto.Password.Equals(registerDto.PasswordAgain))
                {
                    throw new InvalidDataException("Sifreler uyusmuyor!");
                }

                var userIsExist = await _userRepository.GetAll(false).IgnoreQueryFilters().AnyAsync(x => x.UserName == registerDto.UserName);
                if (userIsExist)
                {
                    throw new InvalidDataException("Kullanici adi zaten kullaniliyor!");
                }

                var userIsExistWithEmail = await _userRepository
                    .GetAll(false)
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(x => x.Email == registerDto.Email);

                if (userIsExistWithEmail is not null)
                {
                    if(userIsExistWithEmail.IsDeleted)
                        throw new InvalidDataException("Bu email adresi banlanmıştır!");

                    throw new InvalidDataException("Email adresi zaten kullanılıyor!");
                }

                var facultyIsExist = await _facultRepository.AnyAsync(x => x.Id == registerDto.FacultyId);
                if (!facultyIsExist)
                {
                    throw new InvalidDataException("Fakulte bilgisi girmediniz!");
                }

                var majorIsExist = await _majorRepository.AnyAsync(x => x.Id == registerDto.MajorId);
                if (!majorIsExist)
                {
                    throw new InvalidDataException("Bolum bilgisi girmediniz!");
                }

                var role = await _roleRepository.GetRoleByNameAsync(Roles.User);

                var user = _mapper.Map<User>(registerDto);
                user.PasswordHash = _passwordHasher.Hash(registerDto.Password);

                user.Roles = new List<UserRole>()
                {
                    new()
                    {
                        RoleId = role.Id,
                    }
                };

                var emailVerifyToken = Guid.NewGuid().ToString();
                user.VerifyEmailToken = emailVerifyToken;

                var confirmationLink = GenerateLink(emailVerifyToken, context);

                await _userRepository.CreateAsync(user);

                await _unitOfWork.SaveAsync();

                try
                {
                    var template = new EmailSendTemplate()
                    {
                        To = user.Email,
                        Content = confirmationLink,
                        SendType = SendType.VerifyEmail
                    };

                    await _queue.AddQueue(template);
                }
                catch (Exception)
                {
                    throw;
                }

                return Response<NoContent>.Success("Kayit olma islemi tamamlandi", 202);
            }
            catch (InvalidDataException err)
            {
                _logger.LogInformation(err.Message);
                return Response<NoContent>
                    .Fail(err.Message, 400);
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
                return Response<NoContent>.Fail("Birseyler ters gitti.", 500);
            }
        }

        public async Task<Response<UserIdPwdResetDto>> ForgetPasswordAsync(ForgetPasswordDto forgetPasswordDto)
        {
            int statusCode = StatusCodes.Status200OK;
            try
            {
                User? user = await _userRepository.GetByUserOrEmailAsync(forgetPasswordDto.userNameOrEmail);

                if (user is not User)
                {
                    statusCode = StatusCodes.Status404NotFound;
                    throw new InvalidDataException($"Kullanici Bulunamadi");
                }

                var pswReset = await _pwdResetRepository
                    .GetByCondition(x => x.UserId == user.Id, false)
                    .FirstOrDefaultAsync();

                if (pswReset is not null && pswReset.ExpiresDate > DateTime.Now)
                {
                    statusCode = StatusCodes.Status400BadRequest;
                    throw new InvalidDataException("Henuz yeni kod isteyemezsin");
                }

                var resetCode = GenerateResetCode();
                var expiresDate = DateTime.Now.AddMinutes(3);
                var refCode = GenerateReferansCode();

                if (pswReset is null)
                {
                    pswReset = new PasswordReset();
                }

                pswReset.ExpiresDate = expiresDate;
                pswReset.UserId= user.Id;
                pswReset.RefCode = refCode;
                pswReset.AuthField = resetCode;


                _pwdResetRepository.Update(pswReset);

                await _unitOfWork.SaveAsync();
                try
                {
                    var template = new EmailSendTemplate()
                    {
                        To = user.Email,
                        Content = resetCode+";"+refCode,
                        SendType = SendType.ResetPassword
                    };

                    await _queue.AddQueue(template);
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException(
                        "Mail gonderilirken hata olustu. Lutfen daha sonra tekrar deneyin.");
                }

                return Response<UserIdPwdResetDto>
                    .Success(new(user.Id, refCode, expiresDate), $"Dogrulama kodu basarili bir sekilde gonderildi: {user.Email}"
                        , 200);
            }
            catch (InvalidDataException err)
            {
                _logger.LogWarning(err.Message);
                return Response<UserIdPwdResetDto>
                    .Fail(err.Message, statusCode);
            }
            catch (InvalidOperationException err)
            {
                _logger.LogError(err.Message);
                return Response<UserIdPwdResetDto>
                    .Fail(err.Message, 500);
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
                return Response<UserIdPwdResetDto>
                    .Fail("Bir seyler ters gitti.", 500);
            }
        }

        public async Task<Response<string>> ForgetPasswordVerifyCodeAsync(
               ForgetPasswordWithCodeDto _resetWithCodeDto)
        {
            int statusCode = 200;
            try
            {
                var result = await _userRepository.AnyAsync(x => x.Id == _resetWithCodeDto.userIdentifier);

                if (!result)
                {
                    statusCode = 404;
                    throw new InvalidDataException("Kullanici bulunamadi.");
                }

                var pswReset = await _pwdResetRepository
                    .GetByCondition(x => x.UserId == _resetWithCodeDto.userIdentifier, false)
                    .FirstOrDefaultAsync();

                if (pswReset is null)
                {
                    statusCode = 404;
                    throw new InvalidDataException("Sifirlama kodu bulunamdi.");
                }

                if (pswReset.ExpiresDate < DateTime.Now)
                {
                    statusCode = 400;
                    throw new InvalidDataException("Sifirlama kodunun zamani bitti. Lutfen yeni kod alin.");
                }

                if (!pswReset.AuthField.Equals(_resetWithCodeDto.verifyCode))
                {
                    statusCode = 400;
                    throw new InvalidDataException("Sifirlama kodunu yanlis girdiniz.");
                }

                var token = GeneratePasswordResetTokenAsync();

                pswReset.AuthField = token;
                pswReset.ExpiresDate = DateTime.Now.AddHours(3);

                _pwdResetRepository.Update(pswReset);
                await _unitOfWork.SaveAsync();

                return Response<string>
                    .Success(token, 200);
            }
            catch (InvalidDataException err)
            {
                _logger.LogWarning(err.Message);
                return Response<string>
                    .Fail(err.Message, statusCode);
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
                return Response<string>
                    .Fail("Bir seyler ters gitti.", 500);
            }
        }

        public async Task<Response<NoContent>> ChangePasswordWithResetMethodAsync(ChangePasswordWithResetDto resetRequest)
        {
            int statusCode = 200;
            try
            {
                var user = await _userRepository.GetByIdAsync(resetRequest.userIdentifier);

                if (user is not User)
                {
                    statusCode = 404;
                    throw new InvalidDataException("Kullanici bulunamadi");
                }

                var pswReset = await _pwdResetRepository
                    .GetByCondition(x => x.UserId == user.Id, false)
                    .FirstOrDefaultAsync();

                if (pswReset is null
                    || pswReset.ExpiresDate < DateTime.Now
                    || !pswReset.AuthField.Equals(resetRequest.token))
                {
                    statusCode = 400;
                    throw new InvalidDataException("Gecersiz token");
                }

                if (!resetRequest.newPassword.Equals(resetRequest.newPasswordConfirm))
                {
                    statusCode = 400;
                    throw new InvalidDataException("Sifreler uyusmuyor.");
                }

                if (_passwordHasher.Verify(user.PasswordHash, resetRequest.newPassword))
                {
                    statusCode = 400;
                    throw new InvalidDataException("Eski sifrenizi yeni sifre olarak ayarlayamazsiniz.");
                }

                _pwdResetRepository.Delete(pswReset);

                user.PasswordHash = _passwordHasher.Hash(resetRequest.newPassword);

                _userRepository.Update(user);
                await _unitOfWork.SaveAsync();

                return Response<NoContent>
                    .Success($"Sifreniz basarili bir sekilde degistirildi.", 200);
            }
            catch (InvalidDataException err)
            {
                _logger.LogWarning(err.Message);
                return Response<NoContent>
                    .Fail(err.Message, statusCode);
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
                return Response<NoContent>
                    .Fail("Bir seyler ters gitti.", 500);
            }
        }

        public async Task<Response<NoContent>> LogoutUserAsync(int userId)
        {
            int statusCode = StatusCodes.Status200OK;
            try
            {
                var result = await _userRepository.AnyAsync(x => x.Id == userId);

                if (!result)
                {
                    statusCode = StatusCodes.Status404NotFound;
                    throw new InvalidDataException($"Kullanici bulunamadi. {userId}");
                }

                var userToken = await _tokenRepository
                    .GetByCondition(x => x.UserId == userId, false)
                    .FirstOrDefaultAsync();

                if (userToken is not Token)
                {
                    statusCode = StatusCodes.Status400BadRequest;
                    throw new InvalidDataException($"Kullanici zaten cikis yapti. {userId}");
                }

                _tokenRepository.Delete(userToken);

                await _unitOfWork.SaveAsync();

                return Response<NoContent>
                    .Success($"Cikis yapma islemi basarili bir sekilde gereceksetirildi.", StatusCodes.Status200OK);
            }
            catch (InvalidDataException err)
            {
                _logger.LogWarning(err.Message);
                return Response<NoContent>
                    .Fail(err.Message, statusCode);
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
                return Response<NoContent>
                    .Fail("Bir seyler test gitti.", StatusCodes.Status500InternalServerError);
            }

        }

        public async Task<Response<NoContent>> VerifyEmailRequestAsync(string token)
        {
            try
            {
                var user = await _userRepository.GetByEmailVerifyTokenAsync(token);

                if (user is not User)
                {
                    throw new InvalidDataException($"Kullanici bulunamadi");
                }

                if (string.IsNullOrEmpty(user.VerifyEmailToken)
                    || !user.VerifyEmailToken.Equals(token))
                {
                    throw new InvalidDataException($"Gecersiz anahtar");
                }

                if (user.VerifyEmail)
                {
                    throw new InvalidDataException($"Kullanici zaten dogrulanmis");
                }

                user.VerifyEmail = true;
                _userRepository.Update(user);

                await _unitOfWork.SaveAsync();

                return Response<NoContent>
                    .Success($"Kullanici basarili bir sekilde dogrulandi", 200);
            }
            catch (InvalidDataException err)
            {
                _logger.LogInformation(err.Message);
                return Response<NoContent>
                    .Fail(err.Message, 400);
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
                return Response<NoContent>
                    .Fail("Birseyler ters gitti.", 500);
            }
        }

        private TokenDto CreateToken(User user, Token userToken, bool populateExp)
        {
            var accessToken = _provider.Generate(user);
            // var refreshToken = GenerateRefreshToken();

            // todo Refresh token eklendigi zaman guncellenir
            //if (populateExp)
            //    userToken.RefreshTokenExpires = DateTime.Now.AddDays(7);

            return new(accessToken);
        }

        private string GenerateLink(string token, HttpContext context)
        {
            var verifyLink = _linkGenerator
                .GetUriByAction(
                        context,
                        action: "verify-email",
                        controller: "api",
                        values: null,
                        scheme: context.Request.Scheme
                    );

            return verifyLink+$"/{token}";
        }

        private string GenerateResetCode() =>
               new Random().Next(100000, 999999).ToString();

        private string GenerateReferansCode()
        {
            // A-Z 65 90
            // 0-9 48 50
            Random rnd = new Random();
            int letterCount = 0;
            int digitCount = 0;
            char[] chars = new char[8];
            for (int i = 0; i < 4; i++)
            {
                char ch = (char)rnd.Next(65, 91); // ASCII kodları A-Z aralığında
                int rndLoc;
                do
                {
                    rndLoc = rnd.Next(0, 8);
                } while (chars[rndLoc] != '\0');

                chars[rndLoc] = ch;

                ch = (char)rnd.Next(48, 58); // ASCII kodları 0-9 aralığında
                do
                {
                    rndLoc = rnd.Next(0, 8);
                } while (chars[rndLoc] != '\0');

                chars[rndLoc] = ch;
            }

            return string.Join("", chars);
        }

        public string GeneratePasswordResetTokenAsync()
        {
            byte[] randomBytes = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            string token = Convert.ToBase64String(randomBytes);

            return token;
        }


        //private string GenerateRefreshToken()
        //{
        //    var randomNumber = new byte[64];
        //    using (var rnd = RandomNumberGenerator.Create())
        //    {
        //        rnd.GetBytes(randomNumber);
        //    }

        //    return Convert.ToBase64String(randomNumber);
        //}

    }
}
