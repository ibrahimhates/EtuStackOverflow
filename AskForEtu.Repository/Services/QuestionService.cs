using AskForEtu.Core.Dto.Response;
using AskForEtu.Core.Pagination;
using AskForEtu.Core.ResultStructure;
using AskForEtu.Core.Services;
using AskForEtu.Core.Services.Repo;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AskForEtu.Repository.Services;
public class QuestionService : IQuestionService
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<QuestionService> _logger;
    public QuestionService(IQuestionRepository questionRepository, IMapper mapper, ILogger<QuestionService> logger)
    {
        _questionRepository=questionRepository;
        _mapper=mapper;
        _logger=logger;
    }

    public async Task<Response<List<QuestionListDto>>> GetAllQuestionWithPaggingAsync(int pageNumber)
    {
        try
        {
            var totalCount = await _questionRepository.GetCountAsync();

            var pagger = new Pager()
            {
                TotalCount = totalCount,
                CurrentPage = pageNumber,
            };

            var questions = await _questionRepository
                .GetAll(false)
                .ToPagging(pageNumber, pagger.PageSize)
                .OrderByDescending(x => x.CreatedDate)
                .Include(x => x.User)
                .ToListAsync();

            var questionsDto = _mapper.Map<List<QuestionListDto>>(questions);

            return Response<List<QuestionListDto>>.Success(questionsDto,200,pagger);
        }
        catch (Exception err)
        {
            _logger.LogError(err.Message);
            return Response<List<QuestionListDto>>.Fail("Bir seyler ters gitti.",500);
        }
    }
}
