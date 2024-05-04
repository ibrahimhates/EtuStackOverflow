using Microsoft.AspNetCore.Mvc;

namespace EtuStackOverflow.Controllers
{
    public class QuestionController : Controller
    {
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

        public IActionResult AllQuestion()
        {
            return Ok();
        }
    }

    public class Question
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
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
