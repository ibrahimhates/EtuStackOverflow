using Microsoft.AspNetCore.Mvc;

namespace EtuStackOverflow.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        
        public IActionResult Register()
        {
            return View();
        }
        
        public IActionResult ForgetPassword()
        {
            return View();
        }
    }
}
