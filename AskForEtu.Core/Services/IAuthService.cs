using AskForEtu.Core.Dto.Request;

namespace AskForEtu.Core.Services
{
    public interface IAuthService
    {
        Task Register(RegisterDto registerDto);
    }
}
