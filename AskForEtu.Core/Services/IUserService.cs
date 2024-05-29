using AskForEtu.Core.Dto.Request;
using AskForEtu.Core.Dto.Response;
using AskForEtu.Core.ResultStructure;
using AskForEtu.Core.ResultStructure.Dto;

namespace AskForEtu.Core.Services
{
    public interface IUserService
    {
        Task<Response<UserProfileDto>> UserProfileDetailAsync(int userId);
        Task<Response<NoContent>> UpdateUserProfileDetailAsync(int userId, UserProfileUpdateDto profileUpdateDto);
        Task<Response<List<UserListDto>>> AllUserWithPaggingAsync(int pageNumber, string? searchTerm);
        Task<Response<UserProfileDto>> GetOneUserDetailAsync(int userId, string role);
        Task<Response<NoContent>> DeleteOneUserAsync(int userId);
    }
}
