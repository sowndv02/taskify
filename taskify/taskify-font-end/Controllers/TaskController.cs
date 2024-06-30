using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
using taskify_font_end.Service;
using taskify_font_end.Service.IService;

namespace taskify_font_end.Controllers
{
    public class TaskController : BaseController
    {
        private readonly IWorkspaceService _workspaceService;
        private readonly IUserService _userService;
        private readonly IStatusService _statusService;
        private readonly IColorService _colorService;
        private readonly IPriorityService _priorityService;
        private readonly ITaskService _taskService;
        private readonly IProjectService _projectService;
        private readonly ITaskUserService _taskUserService;
        private readonly IProjectUserService _projectUserService;

        public TaskController(IStatusService statusService,
            IWorkspaceService workspaceService, IUserService userService,
            IPriorityService priorityService, IColorService colorService, 
            ITaskUserService taskUserService, IProjectService projectService, 
            ITaskService taskService, IProjectUserService projectUserService ) : base(workspaceService)
        {
            _taskService = taskService;
            _statusService = statusService;
            _colorService = colorService;
            _workspaceService = workspaceService;
            _userService = userService;
            _priorityService = priorityService;
            _taskUserService = taskUserService;
            _projectService = projectService;
            _projectUserService = projectUserService;
        }


        public async Task<IActionResult> IndexAsync()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("AccessDenied", "Auth");
            if (ViewBag.selectedWorkspaceId == null || ViewBag.selectedWorkspaceId == 0)
            {
                TempData["error"] = "You don't have any workspaces! Please create a workspace first!";
                return RedirectToAction("Create", "Workspace");
            }
            List<StatusDTO> statuses = await GetStatusesByUserIdAsync(userId);

            ViewBag.statuses = statuses;
            return View();
        }

        public IActionResult Update(int id)
        {
            return View();
        }

        public async Task<IActionResult> CreateAsync()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("AccessDenied", "Auth");
            if (ViewBag.selectedWorkspaceId == null || ViewBag.selectedWorkspaceId == 0)
            {
                TempData["error"] = "You don't have any workspaces! Please create a workspace first!";
                return RedirectToAction("Create", "Workspace");
            }
            ViewBag.projects = await GetProjectByUserIdAndWorkspaceIdAsync(userId, ViewBag.selectedWorkspaceId);
            ViewBag.statuses = await GetStatusesByUserIdAsync(userId);

            return View();
        }

        private async Task<List<StatusDTO>> GetStatusesByUserIdAsync(string userId)
        {
            var response = await _statusService.GetByUserIdAsync<APIResponse>(userId);
            List<StatusDTO> list = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                list = JsonConvert.DeserializeObject<List<StatusDTO>>(Convert.ToString(response.Result));
            }

            if (list.Count == 0)
                TempData["warning"] = "No status are available.";

            return list;
        }

        private async Task<List<ProjectDTO>> GetProjectByUserIdAndWorkspaceIdAsync(string userId, int workspaceId)
        {
            var response = await _projectService.GetByUserIdAndWorkspaceIdAsync<APIResponse>(userId, workspaceId);
            List<ProjectDTO> list = new();
            List<ProjectUserDTO> users = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                list = JsonConvert.DeserializeObject<List<ProjectDTO>>(Convert.ToString(response.Result));
            }
            if (list.Count > 0)
            {
                list = list.OrderByDescending(x => x.CreatedDate)
                   .ThenByDescending(x => x.UpdatedDate)
                   .ToList();

                foreach (var item in list)
                {
                    var resUser = await _projectUserService.GetAsync<APIResponse>(item.Id);
                    if (resUser != null && resUser.IsSuccess && resUser.ErrorMessages.Count == 0)
                    {
                        users = JsonConvert.DeserializeObject<List<ProjectUserDTO>>(Convert.ToString(resUser.Result));
                        item.ProjectUsers = users;
                        item.ProjectUserIds = users.Select(x => x.UserId).ToList();
                    }
                }
            }
            return list;
        }

        public async Task<IActionResult> GetUsersByProject(int projectId)
        {
            var resUser = await _projectUserService.GetAsync<APIResponse>(item.Id);
            var users = new List<ProjectUserDTO>();
            if (resUser != null && resUser.IsSuccess && resUser.ErrorMessages.Count == 0)
            {
                users = JsonConvert.DeserializeObject<List<ProjectUserDTO>>(Convert.ToString(resUser.Result));
            }
            var userList = users.Select(u => new { id = u.Id, name = u.User?.FirstName + " " + u.User?.LastName }).ToList();

            return Json(new { users = userList });
        }

    }
}
