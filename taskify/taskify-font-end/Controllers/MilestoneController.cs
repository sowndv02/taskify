using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
using taskify_font_end.Service;
using taskify_font_end.Service.IService;

namespace taskify_font_end.Controllers
{
    public class MilestoneController : Controller
    {
        private readonly IMilestoneService _milestoneService;

        public MilestoneController(IMilestoneService milestoneService)
        {
            _milestoneService = milestoneService;
        }


        [HttpGet]
        public async Task<IActionResult> GetMilestone(int id)
        {
            var result = await _milestoneService.GetAsync<APIResponse>(id);
            var milestone = new MilestoneDTO();
            if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
            {
                milestone = JsonConvert.DeserializeObject<MilestoneDTO>(Convert.ToString(result.Result));
                var milestoneDto = new MilestoneDTO
                {
                    Id = milestone.Id,
                    Title = milestone.Title,
                    StartAt = milestone.StartAt,
                    EndAt = milestone.EndAt,
                    Status = milestone.Status,
                    Progress = milestone.Progress,
                    Description = milestone.Description,
                    UserId = milestone.UserId,
                    CreatedDate = milestone.CreatedDate,
                    UpdatedDate = milestone.UpdatedDate,
                    ProjectId = milestone.ProjectId
                };

                return Json(new { milestone = milestoneDto });
            }
            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> Create(MilestoneDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data." });
            }
            model.CreatedDate = DateTime.Now;   
            var result = await _milestoneService.CreateAsync<APIResponse>(model);

            if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
            {
                return Json(new { success = true, message = "Milestone created successfully!" });
            }

            return Json(new { success = false, message = "Failed to create milestone." });
        }

        [HttpPost]
        public async Task<IActionResult> Update(MilestoneDTO model)
        {
            if (ModelState.IsValid)
            {
                model.UpdatedDate = DateTime.Now;
                var result = await _milestoneService.UpdateAsync<APIResponse>(model);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                {
                    return Json(new { success = true, message = "Milestone updated successfully!" });
                }

                return Json(new { success = false, message = "Failed to update milestone." });
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            return Json(new { success = false, message = errors.First() });
           
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
                APIResponse result = await _milestoneService.DeleteAsync<APIResponse>(id);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                    return Json(new { error = false, message = "Milestone deleted successfully" });
                else
                    return Json(new { error = true, message = result?.ErrorMessages?.FirstOrDefault() ?? "An error occurred while deleting the milestone" });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, message = "An error occurred: " + ex.Message });
            }
        }
    }
}
