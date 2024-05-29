using AskForEtu.Core.Dto.Request;
using AskForEtu.Core.Entity;
using AskForEtu.Core.ResultStructure;
using AskForEtu.Core.ResultStructure.Dto;
using AskForEtu.Core.Services;
using AskForEtu.Core.Services.Repo;
using AskForEtu.Repository.UnitofWork;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AskForEtu.Repository.Services;
public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly ILogger<CommentService> _logger;
    public CommentService(ICommentRepository commentRepository,
        IMapper mapper, IUnitOfWork unitOfWork,
        ILogger<CommentService> logger, IUserRepository userRepository, 
        IQuestionRepository questionRepository)
    {
        _commentRepository=commentRepository;
        _mapper=mapper;
        _unitOfWork=unitOfWork;
        _logger=logger;
        _userRepository=userRepository;
        _questionRepository=questionRepository;
    }

    public async Task<Response<NoContent>> CreateCommentAsync(CreateCommentDto createCommentDto, int userId)
    {
        try
        {
            var userIsExist = await _userRepository.AnyAsync(x => x.Id == userId);
            if (!userIsExist)
                throw new InvalidDataException("Kullanici bulunamadi!");

            var questionIsExist = await _questionRepository.AnyAsync(x => x.Id == createCommentDto.QuestionId);

            if (!questionIsExist)
                throw new InvalidDataException("Soru bulunamadi!");

            var comment = _mapper.Map<Comment>(createCommentDto);
            comment.UserId = userId;

            await _commentRepository.CreateAsync(comment);
            await _unitOfWork.SaveAsync();

            return Response<NoContent>.Success("Soru basarili bir sekilde olusturuldu",204);
        }
        catch(InvalidDataException err)
        {
            _logger.LogError(err.Message);
            return Response<NoContent>.Fail(err.Message,404);
        }
        catch (Exception err)
        {
            _logger.LogError(err.Message);
            return Response<NoContent>.Fail("Birseyler ters giti.", 404);
        }
    }

    public async Task<Response<NoContent>> DeleteCommentByAdminAsync(long id)
    {

        try
        {
            var deletedComment = await _commentRepository.GetByIdAsync(id);

            if (deletedComment is null)
                throw new InvalidDataException("Silmek istediginiz yorum bulunamadi!");

            deletedComment.IsDeleted = true;

            _commentRepository.Update(deletedComment);
            await _unitOfWork.SaveAsync();

            return Response<NoContent>.Success("Yorum basarili bir sekilde silindi", 204);
        }
        catch (InvalidDataException err)
        {
            _logger.LogError(err.Message);
            return Response<NoContent>.Fail(err.Message, 404);
        }
        catch (Exception err)
        {
            _logger.LogError(err.Message);
            return Response<NoContent>.Fail("Birseyler ters giti.", 404);
        }
    }
}
