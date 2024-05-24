using AskForEtu.Core.Dto.Request;
using AskForEtu.Core.Dto.Response;
using AskForEtu.Core.Pagination;
using AskForEtu.Core.ResultStructure;
using AskForEtu.Core.ResultStructure.Dto;
using AskForEtu.Core.Services;
using AskForEtu.Core.Services.Repo;
using AskForEtu.Repository.Services.Repo;
using AskForEtu.Repository.UnitofWork;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AskForEtu.Repository.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;
        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<UserService> logger,
            IUnitOfWork unitOfWork)
        {
            _userRepository=userRepository;
            _mapper=mapper;
            _logger=logger;
            _unitOfWork=unitOfWork;
        }

        public async Task<Response<UserProfileDto>> UserProfileDetailAsync(int userId)
        {
            int statusCode = StatusCodes.Status200OK;
            try
            {
                var user = await _userRepository.GetUserProfileDetail(userId);
                if (user is null)
                {
                    statusCode = StatusCodes.Status404NotFound;
                    throw new InvalidDataException("Kullanici bulunamadi");
                }
                var userDto = _mapper.Map<UserProfileDto>(user);

                return Response<UserProfileDto>.Success(userDto, statusCode);
            }
            catch (InvalidDataException error)
            {
                _logger.LogError(error.Message);
                return Response<UserProfileDto>.Fail(error.Message, statusCode);
            }
            catch (Exception error)
            {
                _logger.LogError(error.Message);
                return Response<UserProfileDto>.Fail("Bir seyler ters gitti", 500);
            }
        }

        public async Task<Response<NoContent>> UpdateUserProfileDetailAsync(int userId,
            UserProfileUpdateDto profileUpdateDto)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user is null)
                {
                    throw new InvalidDataException("Kullanici bulunamadi");
                }
                user = _mapper.Map(profileUpdateDto, user);

                _userRepository.Update(user);

                await _unitOfWork.SaveAsync();

                return Response<NoContent>
                    .Success("Guncelleme islemi basarili bir sekilde gerceklestirildi.",
                        StatusCodes.Status200OK);
            }
            catch (InvalidDataException error)
            {
                _logger.LogError(error.Message);
                return Response<NoContent>.Fail(error.Message, StatusCodes.Status404NotFound);
            }
            catch (Exception error)
            {
                _logger.LogError(error.Message);
                return Response<NoContent>.Fail("Bir seyler ters gitti", 500);
            }
        }

        public async Task<Response<List<UserListDto>>> AllUserWithPaggingAsync(int pageNumber)
        {
            try
            {
                var totalCount = await _userRepository.GetCountAsync();

                var pagger = new Pager()
                {
                    TotalCount = totalCount,
                    CurrentPage = pageNumber,
                };

                var userList = await _userRepository
                    .GetAll(false)
                        .Where(x => x.IsActive && !x.IsDeleted)
                    .ToPagging(pageNumber, pagger.PageSize)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync();

                var userListDto = _mapper.Map<List<UserListDto>>(userList);

                return Response<List<UserListDto>>.Success(userListDto, 200, pagger);
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
                return Response<List<UserListDto>>.Fail("Bir seyler ters gitti.", 500);
            }
        }

        public Task<Response<UserProfileDto>> GetOneUserDetailAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
