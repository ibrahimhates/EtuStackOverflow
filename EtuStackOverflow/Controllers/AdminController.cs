using Microsoft.AspNetCore.Mvc;

namespace EtuStackOverflow.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Users()
        {
            return View();
        }
        
        public IActionResult Report()
        {
            return View();
        }
    }
}
