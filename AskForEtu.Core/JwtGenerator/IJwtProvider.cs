using AskForEtu.Core.Entity;

namespace AskForEtu.Core.JwtGenerator
{
    public interface IJwtProvider
    {
        string Generate(User user);
        Task<(bool, int?)> VerifyTokenAsync(string token);
    }
}
