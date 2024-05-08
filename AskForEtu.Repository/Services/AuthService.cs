using AskForEtu.Core.Dto.Request;
using AskForEtu.Core.Entity;
using AskForEtu.Core.Hash;
using AskForEtu.Core.ResultStructure;
using AskForEtu.Core.ResultStructure.Dto;
using AskForEtu.Core.Services;
using AskForEtu.Core.Services.Repo;
using AskForEtu.Repository.UnitofWork;
using AutoMapper;
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
        public AuthService(
            IUserRepository userRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            ILogger<AuthService> logger,
            IFacultRepository facultRepository,
            IMajorRepository majorRepository)
        {
            _userRepository=userRepository;
            _mapper=mapper;
            _unitOfWork=unitOfWork;
            _passwordHasher=passwordHasher;
            _logger=logger;
            _facultRepository=facultRepository;
            _majorRepository=majorRepository;
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

                var emailVerifyToken = _userRepository.GenerateEmailVerifyToken();
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

        public async Task<Response<NoContent>> VerifyEmailRequestAsync(string token)
        {
            try
            {
                token = token.Replace(" ", "+");

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
    }
}
