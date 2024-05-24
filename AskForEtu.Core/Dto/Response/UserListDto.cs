namespace AskForEtu.Core.Dto.Response
{
    public record UserListDto(
            int Id,
            string FullName,
            byte[]? ProfilePhoto,
            string UserName
        );
}
