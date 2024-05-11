namespace AskForEtu.Core.Dto.Response
{
    public record UserIdPwdResetDto(
            int userIdentifier,
            string refCode,
            DateTime expiresTime
        );
}