using Microsoft.AspNetCore.Mvc;

namespace EtuStackOverflow.Controllers
{
    public class EmailController : Controller
    {
        [Route("/email/islem_basarili")]
        public IActionResult Success()
        {
            return View();
        }

        [Route("/email/islem_basarisiz")]
        public IActionResult UnSuccessfully()
        {
            return View();
        }
    }
}
