using AskForEtu.Core.Behaviour;
using AskForEtu.Core.Dto.Request;
using AskForEtu.Core.Dto.Response;
using AskForEtu.Core.Entity;
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

                userDto.InteractionCount = user.Questions.Sum(q => q.Comments.Count());

                userDto.Questions = _mapper.Map<List<QuestionForProfileDto>>(user.Questions.Take(5));
                userDto.RoleName = user.Roles.First().Role.Name;

                var interactionsComment = user.Questions
                    .SelectMany(q => q.Comments)
                    .OrderByDescending(c => c.CreatedDate)
                    .Take(5);

                userDto.Interactions = _mapper.Map<List<CommentDto>>(interactionsComment);

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

        public async Task<Response<List<UserListDto>>> AllUserWithPaggingAsync(int pageNumber, string? searchTerm)
        {
            try
            {
                var totalCount = await _userRepository.GetCountAsync();

                var pagger = new Pager()
                {
                    TotalCount = totalCount,
                    CurrentPage = pageNumber,
                };

                var query = _userRepository
                    .GetAll(false);

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(x => x.UserName.Contains(searchTerm)
                    || x.Name.Contains(searchTerm)
                    || x.SurName.Contains(searchTerm));
                }

                var userList = await query
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

        public async Task<Response<UserProfileDto>> GetOneUserDetailAsync(int userId, string role)
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

                userDto.InteractionCount = user.Questions.Sum(q => q.Comments.Count());

                userDto.Questions = _mapper.Map<List<QuestionForProfileDto>>(user.Questions.Take(5));
                userDto.RoleName = user.Roles.First().Role.Name;

                List<Comment> comments = null;
                if (role.Equals(Roles.User))
                {
                    comments = user.Questions
                    .SelectMany(q => q.Comments)
                    .OrderByDescending(c => c.CreatedDate)
                    .Take(5).ToList();
                }
                else
                {
                    comments = user.Comments.ToList();
                }

                userDto.Interactions = _mapper.Map<List<CommentDto>>(comments);

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

        public async Task<Response<NoContent>> DeleteOneUserAsync(int userId)
        {
            try
            {
                var user = await _userRepository
                    .GetByCondition(x => x.Id == userId, true)
                    .Include(x => x.Questions)
                    .ThenInclude(x => x.Comments)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    throw new InvalidDataException("Kullanici bulunamadi");
                }

                user.IsDeleted = true;

                foreach (var question in user.Questions)
                {
                    question.IsDeleted = true;

                    foreach (var comment in question.Comments)
                    {
                        comment.IsDeleted = true;
                    }
                }

                _userRepository.Update(user);
                await _unitOfWork.SaveAsync();

                return Response<NoContent>.Success("Kullanıcı silindi", 200);
            }
            catch (InvalidDataException err)
            {
                _logger.LogError(err.Message);
                return Response<NoContent>.Fail(err.Message, 404);
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
                return Response<NoContent>.Fail("Bir seyler ters gitti.", 500);
            }
        }
    }
}
