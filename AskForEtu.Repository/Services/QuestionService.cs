using AskForEtu.Core.Dto.Request;
using AskForEtu.Core.Dto.Response;
using AskForEtu.Core.Entity;
using AskForEtu.Core.Pagination;
using AskForEtu.Core.ResultStructure;
using AskForEtu.Core.ResultStructure.Dto;
using AskForEtu.Core.Services;
using AskForEtu.Core.Services.Repo;
using AskForEtu.Repository.UnitofWork;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;

namespace AskForEtu.Repository.Services;
public class QuestionService : IQuestionService
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<QuestionService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public QuestionService(IQuestionRepository questionRepository,
        IMapper mapper,
        ILogger<QuestionService> logger,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository)
    {
        _questionRepository=questionRepository;
        _mapper=mapper;
        _logger=logger;
        _unitOfWork=unitOfWork;
        _userRepository=userRepository;
    }

    public async Task<Response<NoContent>> CreateQuestionAsync(CreateQuestionDto createQuestionDto, int userId)
    {
        try
        {
            var userExist = await _userRepository
                .GetAll(false)
                .AnyAsync(x => x.Id == userId);

            if (!userExist)
            {
                throw new InvalidDataException("Kullanıcı Bulunamadı");
            }

            var question = _mapper.Map<Question>(createQuestionDto);
            question.UserId = userId;

            await _questionRepository.CreateAsync(question);
            await _unitOfWork.SaveAsync();

            return Response<NoContent>.Success("Soru başarılı bir şekilde oluşturuldu", 200);
        }
        catch (InvalidDataException err)
        {
            _logger.LogError(err.Message);
            return Response<NoContent>.Fail(err.Message, 404);
        }
        catch (Exception err)
        {
            _logger.LogError(err.Message);
            return Response<NoContent>.Fail("Bir seyler ters gitti.", 500);
        }
    }

    public async Task<Response<NoContent>> DeleteQuestionAsync(long id,int userId)
    {
        try
        {
            var deletedQuestion = await _questionRepository.GetByIdAsync(id);

            if (deletedQuestion == null)
            {
                throw new InvalidDataException("Soru bulunamadı");
            }

            if(deletedQuestion.UserId != userId)
            {
                throw new InvalidDataException("Bu işlemi yapmaya yetkiniz yok");
            }

            deletedQuestion.IsDeleted = true;

            _questionRepository.Update(deletedQuestion);
            await _unitOfWork.SaveAsync();

            return Response<NoContent>.Success("Soru silindi", 200);
        }
        catch (InvalidDataException err)
        {
            _logger.LogError(err.Message);
            return Response<NoContent>.Fail(err.Message, 404);
        }
        catch (Exception err)
        {
            _logger.LogError(err.Message);
            return Response<NoContent>.Fail("Bir seyler ters gitti.", 500);
        }
    }

    public async Task<Response<NoContent>> DeleteQuestionByAdminAsync(long id)
    {
        try
        {
            var deletedQuestion = await _questionRepository.GetAll(true)
                .Include(x => x.Comments)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (deletedQuestion == null)
            {
                throw new InvalidDataException("Soru bulunamadı");
            }

            deletedQuestion.IsDeleted = true;

            foreach (var comment in deletedQuestion.Comments)
            {
                comment.IsDeleted = true;
            }

            _questionRepository.Update(deletedQuestion);
            await _unitOfWork.SaveAsync();

            return Response<NoContent>.Success("Soru silindi", 200);
        }
        catch (InvalidDataException err)
        {
            _logger.LogError(err.Message);
            return Response<NoContent>.Fail(err.Message, 404);
        }
        catch (Exception err)
        {
            _logger.LogError(err.Message);
            return Response<NoContent>.Fail("Bir seyler ters gitti.", 500);
        }
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

            return Response<List<QuestionListDto>>.Success(questionsDto, 200, pagger);
        }
        catch (Exception err)
        {
            _logger.LogError(err.Message);
            return Response<List<QuestionListDto>>.Fail("Bir seyler ters gitti.", 500);
        }
    }

    public async Task<Response<QuestionDetailDto>> GetOneQuestionDetailAsync(long id)
    {
        try
        {
            var question = await _questionRepository
                .GetAll(false)
                .Include(x => x.User)
                .Include(x => x.Comments.OrderByDescending(c => c.CreatedDate))
                    .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (question is null)
                throw new InvalidDataException("Soru bulunamadı");

            var questionsDto = _mapper.Map<QuestionDetailDto>(question);

            return Response<QuestionDetailDto>.Success(questionsDto, 200);
        }
        catch (InvalidDataException err)
        {
            _logger.LogError(err.Message);
            return Response<QuestionDetailDto>.Fail(err.Message, 404);
        }
        catch (Exception err)
        {
            _logger.LogError(err.Message);
            return Response<QuestionDetailDto>.Fail("Bir seyler ters gitti.", 500);
        }
    }

    public async Task<Response<NoContent>> MarkSolvedQuestionAsync(long id)
    {
        try
        {
            var question = await _questionRepository.GetByIdAsync(id);

            if (question is null)
                throw new InvalidDataException("Soru bulunamadı");

            question.IsSolved = !question.IsSolved;

            _questionRepository.Update(question);
            await _unitOfWork.SaveAsync();

            return Response<NoContent>.Success("Soru çözüldü", 200);

        }
        catch (InvalidDataException err)
        {
            _logger.LogError(err.Message);
            return Response<NoContent>.Fail(err.Message, 404);
        }
        catch (Exception err)
        {
            _logger.LogError(err.Message);
            return Response<NoContent>.Fail("Bir seyler ters gitti.", 500);
        }
    }
}
