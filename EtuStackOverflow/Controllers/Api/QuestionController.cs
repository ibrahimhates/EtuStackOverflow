using AskForEtu.Core.Dto.Request;
using AskForEtu.Core.Services;
using AutoMapper;
using EtuStackOverflow.Controllers.Api.CustomControllerBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EtuStackOverflow.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]s")]
    public class QuestionController : CustomController
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService=questionService;
        }

        [HttpGet("allForUser/{id:int}")]
        public IActionResult AllQuestionForUser(int id)
        {

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> AllQuestionsAsync([FromQuery]int pageNumber)
        {
            var result = await _questionService.GetAllQuestionWithPaggingAsync(pageNumber);

            return CreateActionResultInstance(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneQuestionAsync([FromRoute(Name = "id")]int id)
        {
            var result = await _questionService.GetOneQuestionDetailAsync(id);

            return CreateActionResultInstance(result);
        }

        [HttpPost,Authorize]
        public async Task<IActionResult> CreateQuestion(CreateQuestionDto createQuestionDto)
        {
            var id = GetUserId();
            int.TryParse(id, out int userId);
            
            var result = await _questionService.CreateQuestionAsync(createQuestionDto,userId);

            return CreateActionResultInstance(result);
        }


        [HttpGet("interactions/{id:int}")]
        public IActionResult AllInteractionForUser(int id)
        {

            return Ok();
        }

    }
}
