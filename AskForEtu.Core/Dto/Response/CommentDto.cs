using AskForEtu.Core.Entity;

namespace AskForEtu.Core.Dto.Response;
public class CommentDto
{
    public long Id { get; set; }
    public int UserId { get; set; }
    public long QuestionId { get; set; }
    public string Content { get; set; }
    public int  LikeCount { get; set; }
    public int DisLikeCount { get; set; }
    public string UserName { get; set; }
    public byte[]? ProfilePhoto { get; set; }
    public DateTime CreatedDate { get; set; }
}
