using AskForEtu.Core.Dto.Response;
using AskForEtu.Core.ResultStructure;

namespace AskForEtu.Core.Services;
public interface IQuestionService
{
    Task<Response<List<QuestionListDto>>> GetAllQuestionWithPaggingAsync(int pageNumber);
}
