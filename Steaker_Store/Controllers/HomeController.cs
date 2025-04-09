using Microsoft.AspNetCore.Mvc;

namespace Steaker_Store.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
