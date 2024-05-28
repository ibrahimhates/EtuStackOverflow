namespace AskForEtu.Core.Dto.Response;
public record QuestionListDto(
        long Id,
        int UserId,
        string Title,
        string Content,
        bool IsSolved
    )
{
    public DateTime CreatedDate { get; set; }
    public string UserName { get; init; }
    public byte[] ProfilePhoto { get; init; }
}
