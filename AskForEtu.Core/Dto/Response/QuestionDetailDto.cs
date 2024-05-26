using AskForEtu.Core.Entity;

namespace AskForEtu.Core.Dto.Response;
public class QuestionDetailDto
{
    public long Id { get; set; }
    public int UserId { get; set; }
    public byte[]? ProfilePhoto{ get; set; }
    public string UserName { get; set; }
    public DateTime CreatedDate { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public List<CommentDto> Comments { get; set; }
}
