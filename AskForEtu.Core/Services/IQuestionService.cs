using AskForEtu.Core.Dto.Request;
using AskForEtu.Core.Dto.Response;
using AskForEtu.Core.ResultStructure;
using AskForEtu.Core.ResultStructure.Dto;

namespace AskForEtu.Core.Services;
public interface IQuestionService
{
    Task<Response<NoContent>> CreateQuestionAsync(CreateQuestionDto createQuestionDto,int userId);
    Task<Response<NoContent>> DeleteQuestionAsync(long id, int userId);
    Task<Response<NoContent>> DeleteQuestionByAdminAsync(long id);
    Task<Response<List<QuestionListDto>>> GetAllQuestionWithPaggingAsync(int pageNumber);
    Task<Response<QuestionDetailDto>> GetOneQuestionDetailAsync(long id);
    Task<Response<NoContent>> MarkSolvedQuestionAsync(long id);
}
