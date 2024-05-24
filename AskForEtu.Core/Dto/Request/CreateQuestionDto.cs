namespace AskForEtu.Core.Dto.Request;
public record CreateQuestionDto
{
    public string Title { get; init; }
    public string Content { get; init; }
}
