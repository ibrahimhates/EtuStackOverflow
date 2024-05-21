namespace AskForEtu.Core.Dto.Response
{
    public record UserListDto(
            int Id,
            int FullName,
            byte[]? ProfilePhoto,
            string UserName
        );
}
