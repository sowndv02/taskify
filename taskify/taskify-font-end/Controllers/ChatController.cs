using Microsoft.AspNetCore.Mvc;

namespace taskify_font_end.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Chat()
        {
            return View();
        }
    }
}
