namespace AskForEtu.Core.Dto.Response;
public record QuestionListDto(
        long Id,
        int UserId,
        string Title,
        string Content,
        DateTime CreatedDate
    )
{
    public string UserName { get; init; }
    public byte[] ProfilePhoto { get; init; }
}
