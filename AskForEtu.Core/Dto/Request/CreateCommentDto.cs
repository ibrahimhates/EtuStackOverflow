namespace AskForEtu.Core.Dto.Request;
public class CreateCommentDto
{
    public long QuestionId { get; set; }
    public string Content { get; set; }
}
