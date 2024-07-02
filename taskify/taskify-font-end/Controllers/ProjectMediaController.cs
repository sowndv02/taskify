using Microsoft.AspNetCore.Mvc;
using taskify_font_end.Models;
using taskify_font_end.Service.IService;

namespace taskify_font_end.Controllers
{
    public class ProjectMediaController : Controller
    {
        private readonly IProjectMediaService _projectMediaService;

        public ProjectMediaController(IProjectMediaService projectMediaService)
        {
            _projectMediaService = projectMediaService;
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                return Json(new { error = true, message = "Invalid ID" });
            }
            try
            {
                APIResponse result = await _projectMediaService.DeleteAsync<APIResponse>(id);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                    return Json(new { error = false, message = "Project Media deleted successfully" });
                else
                    return Json(new { error = true, message = result?.ErrorMessages?.FirstOrDefault() ?? "An error occurred while deleting the project media" });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, message = "An error occurred: " + ex.Message });
            }
        }
    }
}
