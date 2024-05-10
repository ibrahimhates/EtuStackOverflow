using AskForEtu.Core.Dto.Request;
using AskForEtu.Core.Dto.Response;
using AskForEtu.Core.ResultStructure;
using AskForEtu.Core.ResultStructure.Dto;

namespace AskForEtu.Core.Services
{
    public interface IAuthService
    {
        Task<Response<TokenDto>> LoginAsync(LoginDto loginDto);
        Task<Response<NoContent>> RegisterAsync(RegisterDto registerDto);
        Task<Response<NoContent>> LogoutUserAsync(int userId);
        Task<Response<NoContent>> VerifyEmailRequestAsync(string token);
    }
}
