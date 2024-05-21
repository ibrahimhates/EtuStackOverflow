namespace AskForEtu.Core.Dto.Request
{
    public record ForgetPasswordWithCodeDto(
                int userIdentifier,
                string verifyCode,
                string refCode
            );
}
