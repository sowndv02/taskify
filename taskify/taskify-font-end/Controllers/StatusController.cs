using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
using taskify_font_end.Service.IService;

namespace taskify_font_end.Controllers
{
    public class StatusController : BaseController
    {
        private readonly IProjectService _projectService;
        private readonly IWorkspaceService _workspaceService;
        private readonly IUserService _userService;
        private readonly IStatusService _statusService;
        private readonly ITagService _tagService;
        private readonly IProjectUserService _projectUserService;
        private readonly IProjectTagService _projectTagService;
        private readonly IColorService _colorService;
        private readonly ITaskService _taskService;
        private readonly IMapper _mapper;

        public StatusController(IProjectService projectService, IMapper mapper,
            IWorkspaceService workspaceService, IUserService userService,
            IStatusService statusService, ITagService tagService,
            IProjectUserService projectUserService, IProjectTagService projectTagService,
            IConfiguration configuration, IColorService colorService,
            ITaskService taskService) : base(workspaceService)
        {
            _workspaceService = workspaceService;
            _mapper = mapper;
            _projectService = projectService;
            _userService = userService;
            _statusService = statusService;
            _tagService = tagService;
            _projectUserService = projectUserService;
            _projectTagService = projectTagService;
            _colorService = colorService;
            _taskService = taskService;
        }

       
        public async Task<IActionResult> IndexAsync()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("AccessDenied", "Auth");
            try
            {
                List<StatusDTO> list = await GetStatusesByUserIdAsync(userId);
                ViewBag.colors = await GetColorssByUserIdAsync(userId);
                return View(list);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("Dashboard", "Home");
            }
        }

        public async Task<IActionResult> ListAsync()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("AccessDenied", "Auth");
            List<StatusDTO> list = await GetStatusesByUserIdAsync(userId);
            return Json(list);
        }

        public async Task<IActionResult> Get(int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("AccessDenied", "Auth");
            StatusDTO obj = await GetStatusById(id);
            return Json(obj);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(StatusDTO statusDTO)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(userId) || !statusDTO.UserId.Equals(userId))
                {
                    return RedirectToAction("AccessDenied", "Auth");
                }

                APIResponse result = await _statusService.CreateAsync<APIResponse>(statusDTO);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                {
                    TempData["success"] = "Create new status successfully";
                    return RedirectToAction("Index", "Status");
                }
                else
                {
                    TempData["error"] = result.ErrorMessages.FirstOrDefault();
                }
            }
            else
            {
                var errorMessages = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage).FirstOrDefault();
                TempData["error"] = errorMessages;
            }
            return RedirectToAction("Index", "Status");
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
                var taskStatuses = await GetTaskByStatusId(id);
                if (taskStatuses != null && taskStatuses.Count > 0)
                {
                    return Json(new { error = true, message = "Status is being used by some task. Please delete the task before removing the status!" });
                }
                var projectStatuses = await GetProjectByStatusId(id);
                if (projectStatuses != null && projectStatuses.Count > 0)
                {
                    return Json(new { error = true, message = "Status is being used by some project. Please delete the project before removing the status!" });
                }
                APIResponse result = await _statusService.DeleteAsync<APIResponse>(id);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                    return Json(new { error = false, message = "Status deleted successfully" });
                else
                    return Json(new { error = true, message = result?.ErrorMessages?.FirstOrDefault() ?? "An error occurred while deleting the status" });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsync(StatusDTO statusDTO)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(userId) || !statusDTO.UserId.Equals(userId))
                {
                    return RedirectToAction("AccessDenied", "Auth");
                }

                APIResponse result = await _statusService.UpdateAsync<APIResponse>(statusDTO);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                {
                    TempData["success"] = "Update status successfully";
                    return RedirectToAction("Index", "Status");
                }
                else
                {
                    TempData["error"] = result.ErrorMessages.FirstOrDefault();
                }
            }
            else
            {
                var errorMessages = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage).FirstOrDefault();
                TempData["error"] = errorMessages;
            }
            return RedirectToAction("Index", "Status");
        }

        private async Task<List<StatusDTO>> GetStatusesByUserIdAsync(string userId)
        {
            var response = await _statusService.GetByUserIdAsync<APIResponse>(userId);
            List<StatusDTO> list = new();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<StatusDTO>>(Convert.ToString(response.Result));
            }

            if (list.Count == 0)
                TempData["warning"] = "No status are available.";

            return list;
        }

        private async Task<List<ColorDTO>> GetColorssByUserIdAsync(string userId)
        {
            var response = await _colorService.GetByUserIdAsync<APIResponse>(userId);
            List<ColorDTO> list = new();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ColorDTO>>(Convert.ToString(response.Result));
            }

            if (list.Count == 0)
                TempData["warning"] = "No color are available.";

            return list;
        }

        private async Task<StatusDTO> GetStatusById(int id)
        {
            var response = await _statusService.GetAsync<APIResponse>(id);
            StatusDTO obj = new();
            if (response != null && response.IsSuccess)
            {
                obj = JsonConvert.DeserializeObject<StatusDTO>(Convert.ToString(response.Result));
            }

            if (obj == null)
                TempData["error"] = "Status not found";

            return obj;
        }

        private async Task<List<TaskDTO>> GetTaskByStatusId(int id)
        {
            var response = await _taskService.GetByStatusIdAsync<APIResponse>(id);
            List<TaskDTO> obj = new();
            if (response != null && response.IsSuccess)
            {
                obj = JsonConvert.DeserializeObject<List<TaskDTO>>(Convert.ToString(response.Result));
            }
            return obj;
        }

        private async Task<List<ProjectDTO>> GetProjectByStatusId(int id)
        {
            var response = await _projectService.GetByStatusIdAsync<APIResponse>(id);
            List<ProjectDTO> obj = new();
            if (response != null && response.IsSuccess)
            {
                obj = JsonConvert.DeserializeObject<List<ProjectDTO>>(Convert.ToString(response.Result));
            }
            return obj;
        }
    }
}
