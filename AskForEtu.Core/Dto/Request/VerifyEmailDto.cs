namespace AskForEtu.Core.Dto.Request
{
    public record VerifyEmailDto(
            string email,
            string token
        );
}
