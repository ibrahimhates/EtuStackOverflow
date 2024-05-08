using Microsoft.AspNetCore.Mvc;

namespace EtuStackOverflow.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        
        public IActionResult Index()
        {
            return View();
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

        [Route("/profile")]
        public IActionResult Profile(int id)
        {
            return View(id);
        }
    }
}
