using Microsoft.AspNetCore.Mvc;

namespace Task6.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}