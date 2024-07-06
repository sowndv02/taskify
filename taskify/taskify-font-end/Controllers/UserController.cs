using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
using taskify_font_end.Service;
using taskify_font_end.Service.IService;

namespace taskify_font_end.Controllers
{
    public class UserController : BaseController
    {
        private readonly IWorkspaceService _workspaceService;
        private readonly IUserService _userService;
        private readonly IProjectService _projectService;
        private readonly IProjectUserService _projectUserService;
        private readonly ITaskUserService _taskUserService;
        private readonly ITaskService _taskService;
        private readonly IStatusService _statusService;
        private readonly IWorkspaceUserService _workspaceUserService;
        public UserController(IWorkspaceService workspaceService, 
            IUserService userService, IProjectService projectService, 
            IProjectUserService projectUserService, 
            ITaskUserService taskUserService, 
            ITaskService taskService, 
            IWorkspaceUserService workspaceUserService,
            IStatusService statusService) : base(workspaceService, workspaceUserService)
        {
            _workspaceUserService = workspaceUserService;
            _statusService = statusService;
            _taskService = taskService; 
            _taskUserService = taskUserService;
            _projectUserService = projectUserService;
            _projectService = projectService;
            _workspaceService = workspaceService;
            _userService = userService;

        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Profile(string id)
        {
            var user = await GetUserByIdAsync(id);

            var projects = await GetProjectByUserIdAndWorkspaceIdAsync(id, (int)ViewBag.SelectedWorkspaceId);
            var tasks = projects.SelectMany(x => x.Tasks).ToList();
            ViewBag.projects = projects;
            ViewBag.tasks = tasks;
            return View(user);
        }

        private async Task<UserDTO> GetUserByIdAsync(string userId)
        {
            var response = await _userService.GetAsync<APIResponse>(userId);
            UserDTO user = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                user = JsonConvert.DeserializeObject<UserDTO>(Convert.ToString(response.Result));
            }
            return user;
        }

        private async Task<List<ProjectDTO>> GetProjectByUserIdAndWorkspaceIdAsync(string userId, int workspaceId)
        {
            var response = await _projectService.GetByUserIdAndWorkspaceIdAsync<APIResponse>(userId, workspaceId);
            List<ProjectDTO> list = new();
            List<TaskDTO> tasks = new();
            List<ProjectUserDTO> users = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                list = JsonConvert.DeserializeObject<List<ProjectDTO>>(Convert.ToString(response.Result));
                var listContainsUser = await GetListProjectContainsUserInWorkspace(userId, workspaceId);

                list = list.Concat(listContainsUser)
                              .GroupBy(p => p.Id)
                              .Select(g => g.First())
                              .ToList()
                              .OrderByDescending(x => x.CreatedDate)
                              .ThenByDescending(x => x.UpdatedDate)
                              .ToList();
                foreach (var item in list)
                {
                    var resUser = await _projectUserService.GetByProjectIdAsync<APIResponse>(item.Id);
                    if (resUser != null && resUser.IsSuccess && resUser.ErrorMessages.Count == 0)
                    {
                        users = JsonConvert.DeserializeObject<List<ProjectUserDTO>>(Convert.ToString(resUser.Result));
                        foreach (var u in users)
                        {
                            u.User = await GetUserByIdAsync(u.UserId);
                        }
                        item.ProjectUsers = users;
                    }

                    var statuses = await GetTaskByProjectIdAndUserIdAsync(userId, item.Id);
                    foreach (var stat in statuses)
                    {
                        item.Tasks.AddRange(stat.Tasks);
                    }
                    item.Tasks.OrderByDescending(x => x.CreatedDate).ThenByDescending(x => x.UpdatedDate);
                }
            }

            return list;
        }

        private async Task<List<ProjectDTO>> GetListProjectContainsUserInWorkspace(string userId, int workspaceId)
        {
            var projects = await GetProjectInWorkspace(workspaceId);
            var projectsHaveUser = await GetListProjectContainsUser(userId, workspaceId);

            return projects.Where(p => projectsHaveUser.Any(pu => pu.Id == p.Id)).ToList();
        }

        private async Task<List<StatusDTO>> GetTaskByProjectIdAndUserIdAsync(string userId, int projectId)
        {
            var response = await _statusService.GetByUserIdAsync<APIResponse>(userId);
            List<StatusDTO> list = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                list = JsonConvert.DeserializeObject<List<StatusDTO>>(Convert.ToString(response.Result));
            }

            if (list.Count > 0)
            {
                list = list
                .Take(4)
                .OrderByDescending(x => x.Id)
                .Concat(
                    list.Skip(4)
                        .OrderBy(x => x.Id)
                )
                .ToList();
                foreach (var item in list)
                {
                    item.Tasks = await GetTaskByProjectIdAndStatusId(projectId, item.Id);
                }
            }

            return list;
        }

        private async Task<List<ProjectDTO>> GetProjectInWorkspace(int workspaceId)
        {
            var response = await _projectService.GetByWorkspaceIdAsync<APIResponse>(workspaceId);
            List<ProjectDTO> list = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                list = JsonConvert.DeserializeObject<List<ProjectDTO>>(Convert.ToString(response.Result));
                foreach (var item in list)
                {
                    var resProjectUsers = await _projectUserService.GetAsync<APIResponse>(item.Id);
                    if (resProjectUsers != null && resProjectUsers.IsSuccess && resProjectUsers.ErrorMessages.Count == 0)
                    {
                        item.ProjectUsers = JsonConvert.DeserializeObject<List<ProjectUserDTO>>(Convert.ToString(resProjectUsers.Result));
                        foreach (var user in item.ProjectUsers)
                        {
                            user.User = await GetUserByIdAsync(user.UserId);
                        }
                        item.ProjectUserIds = item.ProjectUsers.Select(x => x.UserId).ToList();
                    }
                }
            }
            return list;
        }

        private async Task<List<ProjectDTO>> GetListProjectContainsUser(string userId, int workspaceId)
        {
            var response = await _projectUserService.GetByUserIdAsync<APIResponse>(userId);
            List<ProjectUserDTO> list = new();
            List<ProjectDTO> projects = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                list = JsonConvert.DeserializeObject<List<ProjectUserDTO>>(Convert.ToString(response.Result));
                foreach (var item in list)
                {
                    projects.Add(await GetProjectByIdAsync(item.ProjectId));
                }
            }
            return projects;
        }
        private async Task<List<TaskDTO>> GetTaskByProjectIdAndStatusId(int projectId, int id)
        {
            var response = await _taskService.GetByStatusIdAndProjectIdAsync<APIResponse>(projectId, id);
            List<TaskDTO> list = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                list = JsonConvert.DeserializeObject<List<TaskDTO>>(Convert.ToString(response.Result));
            }
            if (list.Count > 0)
            {
                list = list.OrderByDescending(x => x.CreatedDate)
                   .ThenByDescending(x => x.UpdatedDate)
                   .ToList();
                var users = new List<TaskUserDTO>();
                foreach (var item in list)
                {
                    item.Project = await GetProjectByIdAsync(item.ProjectId);
                    item.Status = await GetStatusById(item.StatusId);
                    var resUser = await _taskUserService.GetByTaskIdAsync<APIResponse>(item.Id);
                    if (resUser != null && resUser.IsSuccess && resUser.ErrorMessages.Count == 0)
                    {
                        users = JsonConvert.DeserializeObject<List<TaskUserDTO>>(Convert.ToString(resUser.Result));
                        foreach (var u in users)
                        {
                            u.User = await GetUserByIdAsync(u.UserId);
                        }
                        item.TaskUsers = users;
                    }
                }
            }
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
            return obj;
        }

        private async Task<ProjectDTO> GetProjectByIdAsync(int id)
        {
            var response = await _projectService.GetAsync<APIResponse>(id);
            ProjectDTO obj = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                obj = JsonConvert.DeserializeObject<ProjectDTO>(Convert.ToString(response.Result));
            }

            if (obj.Id != 0)
            {
                var resProjectUsers = await _projectUserService.GetAsync<APIResponse>(obj.Id);
                if (resProjectUsers != null && resProjectUsers.IsSuccess && resProjectUsers.ErrorMessages.Count == 0)
                {
                    obj.ProjectUsers = JsonConvert.DeserializeObject<List<ProjectUserDTO>>(Convert.ToString(resProjectUsers.Result));
                    foreach (var user in obj.ProjectUsers)
                    {
                        user.User = await GetUserByIdAsync(user.UserId);
                    }
                    obj.ProjectUserIds = obj.ProjectUsers.Select(x => x.UserId).ToList();
                }
            }

            return obj;
        }
    }
}
