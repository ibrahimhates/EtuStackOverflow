namespace AskForEtu.Core.Dto.Request
{
    public record LoginDto(
            string userNameOrEmail,
            string password
        );
}
