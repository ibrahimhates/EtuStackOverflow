using AskForEtu.Core.Dto.Request;
using AskForEtu.Core.Dto.Response;
using AskForEtu.Core.Entity;
using AskForEtu.Core.Hash;
using AskForEtu.Core.JwtGenerator;
using AskForEtu.Core.ResultStructure;
using AskForEtu.Core.ResultStructure.Dto;
using AskForEtu.Core.Services;
using AskForEtu.Core.Services.Repo;
using AskForEtu.Repository.UnitofWork;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AskForEtu.Repository.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<AuthService> _logger;
        private readonly IFacultRepository _facultRepository;
        private readonly IMajorRepository _majorRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly IJwtProvider _provider;
        public AuthService(
            IUserRepository userRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            ILogger<AuthService> logger,
            IFacultRepository facultRepository,
            IMajorRepository majorRepository,
            ITokenRepository tokenRepository,
            IJwtProvider provider)
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
        }

        public async Task<Response<TokenDto>> LoginAsync(LoginDto loginDto)
        {
            int statusCode = StatusCodes.Status200OK;
            try
            {
                var user = await _userRepository.GetByUserOrEmailAsync(loginDto.userNameOrEmail);

                if (user is not User)
                {
                    statusCode = StatusCodes.Status404NotFound;
                    throw new InvalidDataException($"User could not found. ");
                }

                if (!_passwordHasher.Verify(user.PasswordHash, loginDto.password))
                {
                    statusCode = StatusCodes.Status401Unauthorized;
                    throw new InvalidDataException($"User password does not matching. {user.Email}");
                }

                if (!user.VerifyEmail)
                {
                    statusCode = StatusCodes.Status401Unauthorized;
                    throw new InvalidDataException("Pending email verification. Please verify your email address");
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
                    .Fail(err.Message, statusCode);
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
                return Response<TokenDto>
                    .Fail("Bir seyler ters gitti.", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<Response<NoContent>> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                if (!registerDto.Password.Equals(registerDto.PasswordAgain))
                {
                    throw new InvalidDataException("Sifreler uyusmuyor!");
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

                var user = _mapper.Map<User>(registerDto);
                user.PasswordHash = _passwordHasher.Hash(registerDto.Password);

                var emailVerifyToken = Guid.NewGuid().ToString();
                user.VerifyEmailToken = emailVerifyToken;

                // var confirmationLink = GenerateLink(emailVerifyToken);

                await _userRepository.CreateAsync(user);

                await _unitOfWork.SaveAsync();

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
