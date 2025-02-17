﻿using AskForEtu.Core.Dto.Request;
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

        [HttpGet]
        public async Task<IActionResult> AllQuestionsAsync([FromQuery]int pageNumber, [FromQuery] string? searchTerm)
        {
            var result = await _questionService.GetAllQuestionWithPaggingAsync(pageNumber,searchTerm);

            return CreateActionResultInstance(result);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetOneQuestionAsync([FromRoute(Name = "id")]long id)
        {
            var result = await _questionService.GetOneQuestionDetailAsync(id);

            return CreateActionResultInstance(result);
        }

        [HttpPost,Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> CreateQuestion(CreateQuestionDto createQuestionDto)
        {
            var id = GetUserId();
            int.TryParse(id, out int userId);
            
            var result = await _questionService.CreateQuestionAsync(createQuestionDto,userId);

            return CreateActionResultInstance(result);
        }

        [HttpDelete("{id:long}"),Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> DeleteQuestion([FromRoute]long id)
        {
            var userId = int.Parse(GetUserId());
            var result = await _questionService.DeleteQuestionAsync(id,userId);

            return CreateActionResultInstance(result);
        }

        [HttpDelete("force/{id:long}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteQuestionByAdmin([FromRoute] long id)
        {
            var result = await _questionService.DeleteQuestionByAdminAsync(id);

            return CreateActionResultInstance(result);
        }

        [HttpPut("solved/{id:long}"),Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> UpdateQuestion([FromRoute]long id)
        {
            var result = await _questionService.MarkSolvedQuestionAsync(id);

            return CreateActionResultInstance(result);
        }
    }
}
