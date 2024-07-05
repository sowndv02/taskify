using Microsoft.AspNetCore.Mvc;
using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
using taskify_font_end.Service;
using taskify_font_end.Service.IService;

namespace taskify_font_end.Controllers
{
    public class TaskMediaController : Controller
    {
        private readonly ITaskMediaService _taskMediaService;

        public TaskMediaController(ITaskMediaService taskMediaService)
        {
            _taskMediaService = taskMediaService;
        }


        [HttpPost]
        public async Task<IActionResult> Upload(List<IFormFile> media_files, string id, string userId)
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
                        var media = new TaskMediaDTO()
                        {
                            UserId = userId,
                            TaskId = int.Parse(id),
                            File = file,
                            FileName = fileName,
                            FileSize = (int)file.Length / 1024.0
                        };
                        var result = await _taskMediaService.CreateAsync<APIResponse>(media);
                        if (result == null || !result.IsSuccess || result.ErrorMessages.Count != 0)
                        {
                            return StatusCode(int.Parse(result.StatusCode.ToString()), new { message = result.ErrorMessages[0] });
                        }
                    }
                }

                return Ok(new { is_error = false, message = "Files successfully uploaded." });
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
                APIResponse result = await _taskMediaService.DeleteAsync<APIResponse>(id);

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
