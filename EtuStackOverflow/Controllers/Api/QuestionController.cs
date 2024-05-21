using AskForEtu.Core.Services;
using EtuStackOverflow.Controllers.Api.CustomControllerBase;
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
            var questionList = new List<Question>();

            for (int i = 1; i <= 5; i++)
            {
                Question question = new Question
                {
                    Id = i,
                    Title = $"Soru {i}",
                    Description = $"Bu, {i}. sorunun açıklamasıdır.",
                    CreatedDate = DateTime.Now.AddMinutes(i)
                };

                questionList.Add(question);
            }

            return Ok(questionList);
        }

        [HttpGet]
        public async Task<IActionResult> AllQuestionsAsync([FromQuery]int pageNumber)
        {
            var result = await _questionService.GetAllQuestionWithPaggingAsync(pageNumber);

            return CreateActionResultInstance(result);
        }

        [HttpGet("one/{id:int}")]
        public IActionResult GetOneQuestion([FromRoute(Name = "id")]int id)
        {
            return Ok(new Question { Id = id });
        }

        [HttpGet("interactions/{id:int}")]
        public IActionResult AllInteractionForUser(int id)
        {
            Random random = new Random();
            var commentList = new List<Comment>();

            for (int i = 1; i <= 5; i++) // varsayim
            {
                commentList.Add(new()
                {
                    Id = i,
                    Name = $"Random Name_{i}",
                    Content = $"Bu, {i}. yorum.",
                    CreatedDate = DateTime.Now.AddMinutes(random.Next(0, 100)),
                    DisLikeCount = random.Next(0, 10),
                    LikeCount = random.Next(10, 20),
                    ProfilePicture = random.Next(1, 9)
                });
            }

            return Ok(commentList);
        }

    }

    public class Question
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    
    public class QuestionAll
    {
        public int id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public string username { get; set; }
        public string pp { get; set; }
    }

    public class Comment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProfilePicture { get; set; }
        public DateTime CreatedDate { get; set; }
        public int LikeCount { get; set; }
        public int DisLikeCount { get; set; }
        public string Content { get; set; }
    }
}
