using AskForEtu.Core.Dto.Request;
using AskForEtu.Core.Services;
using EtuStackOverflow.Controllers.Api.CustomControllerBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EtuStackOverflow.Controllers.Api;
[Route("api/[controller]s")]
[ApiController]
public class CommentController : CustomController
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPost, Authorize]
    public async Task<IActionResult> CreateComment(CreateCommentDto createCommentDto)
    {
        var id = GetUserId();
        int.TryParse(id, out int userId);

        var result = await _commentService.CreateCommentAsync(createCommentDto, userId);

        return CreateActionResultInstance(result);
    }

    [HttpGet("allForUser/{id:int}")]
    public IActionResult AllCommentForUser(int id)
    {
        return Ok();
    }

    [HttpGet("interactions/{id:int}")]
    public IActionResult AllInteractionForUser(int id)
    {
        return Ok();
    }
}
