using AskForEtu.Core.Services.Repo;
using AskForEtu.Repository.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EtuStackOverflow.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IQuestionRepository _questionRepository;
        public HomeController(ILogger<HomeController> logger, IUserRepository userRepository, ICommentRepository commentRepository, IQuestionRepository questionRepository)
        {
            _logger = logger;
            _userRepository=userRepository;
            _commentRepository=commentRepository;
            _questionRepository=questionRepository;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var questionCount = await _questionRepository.GetCountAsync();
            var solvedQuestionCount = await _questionRepository.GetAll(false).Where(x => x.IsSolved).CountAsync();
            var userCount = await _userRepository.GetCountAsync();

            return View(new { solvedQuestionCount, questionCount, userCount });
        }

        [Route("/questions")]
        public IActionResult Questions()
        {
            return View();
        }

        [Route("/questions/{questionId:int}")]
        public IActionResult QuestionDetail()
        {
            return View();
        }

        [Route("/users")]
        public IActionResult Users()
        {
            return View();
        }

        [Route("/users/{userId:int}")]
        public IActionResult UserDetail()
        {
            return View();
        }

        [Route("/profile")]
        public IActionResult Profile(int id)
        {
            return View(id);
        }
    }
}
