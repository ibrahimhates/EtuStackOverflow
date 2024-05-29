
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
        public List<CommentDto>? Interactions { get; set; }
        public List<QuestionForProfileDto>?  Questions { get; set; }
        public string RoleName { get; set; }
        public int CommentCount { get; set; }
        public int InteractionCount { get; set; }
    }
}
