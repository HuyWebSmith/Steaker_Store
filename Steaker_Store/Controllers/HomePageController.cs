using Microsoft.AspNetCore.Mvc;

namespace Steaker_Store.Controllers
{
    public class HomePageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
