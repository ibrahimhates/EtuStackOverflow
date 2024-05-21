
namespace AskForEtu.Core.Dto.Response
{
    public record UserProfileDto(
            int Id,
            string FullName,
            string Name,
            string SurName,
            DateTime? DateOfBirth,
            byte[]? ProfilePhoto,
            string Email,
            bool VerifyEmail,
            string UserName
        )
    {
        public int CommentCount { get; set; }
        public int InteractionCount { get; set; }
    }
}
