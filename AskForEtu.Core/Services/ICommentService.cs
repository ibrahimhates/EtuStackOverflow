using AskForEtu.Core.Dto.Request;
using AskForEtu.Core.ResultStructure;
using AskForEtu.Core.ResultStructure.Dto;

namespace AskForEtu.Core.Services;
public interface ICommentService
{
    Task<Response<NoContent>> CreateCommentAsync(CreateCommentDto createCommentDto, int userId);
}
