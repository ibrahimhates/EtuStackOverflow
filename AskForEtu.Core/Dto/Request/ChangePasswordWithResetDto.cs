namespace AskForEtu.Core.Dto.Request
{
    public record ChangePasswordWithResetDto(
            int userIdentifier,
            string token,
            string newPassword,
            string newPasswordConfirm
        );
}
