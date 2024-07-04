using Microsoft.AspNetCore.Mvc;
using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
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


        [HttpPost]
        public async Task<IActionResult> Upload(List<IFormFile> media_files, string projectId, string userId)
        {
            try
            {
                if (media_files == null || media_files.Count == 0)
                {
                    return BadRequest("No files uploaded.");
                }

                foreach (var file in media_files)
                {
                    if (file.Length > 0)
                    {
                        string fileName = $"{userId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        var projectMedia = new ProjectMediaDTO() { 
                            UserId = userId, 
                            ProjectId = int.Parse(projectId), 
                            File = file, 
                            FileName = fileName, 
                            FileSize = file.Length 
                        };
                        var result = await _projectMediaService.CreateAsync<APIResponse>(projectMedia);
                        if (result == null || !result.IsSuccess || result.ErrorMessages.Count != 0)
                        {
                            return StatusCode(int.Parse(result.StatusCode.ToString()), new { message = result.ErrorMessages[0] }); 
                        }
                    }
                }

                return Ok(new { is_error = false,  message = "Files successfully uploaded." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
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
