namespace AskForEtu.Core.Dto.Request
{
    public record UserProfileUpdateDto(
            string Name,
            string SurName,
            DateTime? DateOfBirth,
            byte[]? ProfilePhoto,
            string UserName
        );
}
