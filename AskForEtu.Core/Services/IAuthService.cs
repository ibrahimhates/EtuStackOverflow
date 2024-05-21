using AskForEtu.Core.Dto.Request;
using AskForEtu.Core.Dto.Response;
using AskForEtu.Core.ResultStructure;
using AskForEtu.Core.ResultStructure.Dto;
using Microsoft.AspNetCore.Http;

namespace AskForEtu.Core.Services
{
    public interface IAuthService
    {
        Task<Response<TokenDto>> LoginAsync(LoginDto loginDto);
        Task<Response<NoContent>> RegisterAsync(RegisterDto registerDto, HttpContext context);
        Task<Response<UserIdPwdResetDto>> ForgetPasswordAsync(ForgetPasswordDto forgetPasswordDto);
        Task<Response<string>> ForgetPasswordVerifyCodeAsync(ForgetPasswordWithCodeDto _resetWithCodeDto);
        Task<Response<NoContent>> ChangePasswordWithResetMethodAsync(ChangePasswordWithResetDto resetRequest);
        Task<Response<NoContent>> LogoutUserAsync(int userId);
        Task<Response<NoContent>> VerifyEmailRequestAsync(string token);
    }
}
