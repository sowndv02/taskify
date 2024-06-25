using Microsoft.AspNetCore.Mvc;

namespace taskify_font_end.Controllers
{
    public class NoteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
