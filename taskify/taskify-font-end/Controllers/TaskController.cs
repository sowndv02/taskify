using Microsoft.AspNetCore.Mvc;

namespace taskify_font_end.Controllers
{
    public class TaskController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Update(int id)
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
