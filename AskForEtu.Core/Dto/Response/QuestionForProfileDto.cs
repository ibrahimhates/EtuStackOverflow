namespace AskForEtu.Core.Dto.Response;
public class QuestionForProfileDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public bool IsSolved { get; set; }
    public DateTime CreatedDate { get; set; }
}
