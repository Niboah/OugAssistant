using Microsoft.AspNetCore.Mvc;

namespace OugAssistant_WEB.Controllers.web
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
