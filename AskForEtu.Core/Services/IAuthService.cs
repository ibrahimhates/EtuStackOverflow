using AskForEtu.Core.Dto.Request;
using AskForEtu.Core.ResultStructure;
using AskForEtu.Core.ResultStructure.Dto;

namespace AskForEtu.Core.Services
{
    public interface IAuthService
    {
        Task<Response<NoContent>> RegisterAsync(RegisterDto registerDto);
        Task<Response<NoContent>> VerifyEmailRequestAsync(string token);
    }
}
