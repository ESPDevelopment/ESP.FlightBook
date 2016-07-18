using Microsoft.AspNetCore.Mvc;

namespace ESP.FlightBook.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
