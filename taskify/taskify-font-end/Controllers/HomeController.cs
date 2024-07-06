using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
using taskify_font_end.Service;
using taskify_font_end.Service.IService;

namespace taskify_font_end.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IWorkspaceService _workspaceService;
        private readonly IProjectService _projectService;
        private readonly IProjectUserService _projectUserService;
        private readonly ITaskUserService _taskUserService;
        private readonly IUserService _userService;
        private readonly ITaskService _taskService;
        private readonly IPriorityService _priorityService;
        private readonly IStatusService _statusService;
        private readonly IWorkspaceUserService _workspaceUserService;
        private readonly ITodoService _todoService;
        private readonly IMapper _mapper;
        public HomeController(IWorkspaceService workspaceService, IMapper mapper, 
            IProjectService projectService, IProjectUserService projectUserService, 
            IUserService userService, ITaskService taskService, 
            ITaskUserService taskUserService, 
            ITodoService todoService,
            IPriorityService priorityService,
            IStatusService statusService, 
            IWorkspaceUserService workspaceUserService) : base(workspaceService, workspaceUserService)
        {
            _workspaceUserService = workspaceUserService;
            _priorityService = priorityService;
            _todoService = todoService;
            _statusService = statusService;
            _taskUserService = taskUserService;
            _taskService = taskService;
            _workspaceService = workspaceService;
            _mapper = mapper;
            _projectService = projectService;
            _projectUserService = projectUserService;
            _userService = userService;
        }

        public IActionResult LandingPage()
        {
            return View();
        }


        public async Task<IActionResult> DashboardAsync(int? id)
        {
            if(id != null && id != 0)
            {
                HttpContext.Response.Cookies.Append("SelectedWorkspaceId", id.ToString());
                ViewBag.SelectedWorkspaceId = id;
            }
                
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("AccessDenied", "Auth");
            }
            List<WorkspaceDTO> workspaces = new();
            if (!string.IsNullOrEmpty(userId))
            {
                workspaces = await GetWorkspaceByUserIdAsync(userId);
                ViewBag.workspaces = workspaces;
                if ((id == null || id == 0) && (ViewBag.SelectedWorkspaceId == null || ViewBag.SelectedWorkspaceId == 0))
                {
                    if (workspaces.Count > 0)
                    {
                        var projects = await GetProjectByUserIdAndWorkspaceIdAsync(userId, workspaces.First().Id);
                        var tasks = projects.SelectMany(x => x.Tasks).ToList();
                        var statuses = await GetStatusesByUserIdAsync(userId);
                        Dictionary<StatusDTO, int> taskDictionary = new Dictionary<StatusDTO, int>();
                        Dictionary<StatusDTO, int> projectDictionary = new Dictionary<StatusDTO, int>();
                        List<TodoDTO> todos = await GetTodoByUserIdAndWorksplaceIdAsync(userId, workspaces.First().Id);
                        ViewBag.todos = todos.Take(5).ToList();
                        foreach (var stat in statuses)
                        {
                            projectDictionary.Add(stat, projects.Where(x => x.StatusId == stat.Id).ToList().Count);
                            taskDictionary.Add(stat, tasks.Where(x => x.StatusId == stat.Id).ToList().Count);
                        }
                        ViewBag.priorities = await GetPrioritiesByUserIdAsync(userId);
                        if (todos.Count > 0)
                        {
                            ViewBag.todoStatistics = (int)todos.Where(x => x.Status).ToList().Count;
                            ViewBag.todoTotal = todos.Count;
                        }
                        else
                        {
                            ViewBag.todoTotal = todos.Count;
                            ViewBag.todoStatistics = 0;
                        }
                        ViewBag.taskStatistics = taskDictionary;
                        ViewBag.projectStatistics = projectDictionary;
                        ViewBag.projects = projects;
                        ViewBag.tasks = tasks;
                        return View(workspaces.First());
                    }
                    return View(null);
                }
                var workspace = workspaces.FirstOrDefault(x => x.Id == ViewBag.SelectedWorkspaceId);
                if(workspace != null)
                {
                    var projects = await GetProjectByUserIdAndWorkspaceIdAsync(userId, workspace.Id);
                    var statuses = await GetStatusesByUserIdAsync(userId);
                    Dictionary<StatusDTO, int> taskDictionary = new Dictionary<StatusDTO, int>();
                    Dictionary<StatusDTO, int> projectDictionary = new Dictionary<StatusDTO, int>();
                    var tasks = projects.SelectMany(x => x.Tasks).ToList();
                    List<TodoDTO> todos = await GetTodoByUserIdAndWorksplaceIdAsync(userId, workspaces.First().Id);
                    foreach (var stat in statuses)
                    {
                        projectDictionary.Add(stat, projects.Where(x => x.StatusId == stat.Id).ToList().Count);
                        taskDictionary.Add(stat, tasks.Where(x => x.StatusId == stat.Id).ToList().Count);
                    }
                    ViewBag.todos = todos.Take(5).ToList();
                    ViewBag.priorities = await GetPrioritiesByUserIdAsync(userId);
                    if (todos.Count > 0)
                    {
                        ViewBag.todoStatistics = (int)todos.Where(x => x.Status).ToList().Count;
                        ViewBag.todoTotal = todos.Count;
                    }
                    else
                    {
                        ViewBag.todoTotal = todos.Count;
                        ViewBag.todoStatistics = 0;
                    }
                    ViewBag.projects = projects;
                    ViewBag.taskStatistics = taskDictionary;
                    ViewBag.projectStatistics = projectDictionary;
                    ViewBag.tasks = tasks;
                }
                return View(workspace);
            }
            return View(null);
        }

        private async Task<List<PriorityDTO>> GetPrioritiesByUserIdAsync(string userId)
        {
            var response = await _priorityService.GetByUserIdAsync<APIResponse>(userId);
            List<PriorityDTO> list = new();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<PriorityDTO>>(Convert.ToString(response.Result));
            }

            if (list.Count == 0)
                TempData["warning"] = "No priority are available.";

            return list;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<List<WorkspaceDTO>> GetWorkspaceByUserIdAsync(string userId)
        {
            var response = await _workspaceService.GetByUserIdAsync<APIResponse>(userId);
            List<WorkspaceDTO> workspaces = new();
            if (response != null && response.IsSuccess)
            {
                workspaces = JsonConvert.DeserializeObject<List<WorkspaceDTO>>(Convert.ToString(response.Result));
                var listContainsUser = await GetListWorkspaceContainsUser(userId);
                workspaces = workspaces.Concat(listContainsUser)
                              .GroupBy(p => p.Id)
                              .Select(g => g.First())
                              .ToList()
                              .OrderByDescending(x => x.CreatedDate)
                              .ThenByDescending(x => x.UpdatedDate)
                              .ToList();
            }
            return workspaces;
        }

        private async Task<List<TodoDTO>> GetTodoByUserIdAndWorksplaceIdAsync(string userId, int workspaceId)
        {
            var response = await _todoService.GetByUserIdAndWorkspaceIdAsync<APIResponse>(userId, workspaceId);
            List<TodoDTO> list = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                list = JsonConvert.DeserializeObject<List<TodoDTO>>(Convert.ToString(response.Result));
                list = list.OrderByDescending(x => x.CreatedDate)
                   .ThenByDescending(x => x.UpdatedDate)
                   .ToList();
            }
            return list;
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
                foreach(var item in list) 
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
                    foreach(var stat in statuses)
                    {
                        item.Tasks.AddRange(stat.Tasks);
                    }
                    item.Tasks.OrderByDescending(x => x.CreatedDate).ThenByDescending(x => x.UpdatedDate);
                }
            }
            
            return list;
        }

        private async Task<List<StatusDTO>> GetStatusesByUserIdAsync(string userId)
        {
            var response = await _statusService.GetByUserIdAsync<APIResponse>(userId);
            List<StatusDTO> list = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                list = JsonConvert.DeserializeObject<List<StatusDTO>>(Convert.ToString(response.Result));
            }

            return list;
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
        private async Task<List<ProjectDTO>> GetListProjectContainsUserInWorkspace(string userId, int workspaceId)
        {
            var projects = await GetProjectInWorkspace(workspaceId);
            var projectsHaveUser = await GetListProjectContainsUser(userId, workspaceId);
            return projects.Where(p => projectsHaveUser.Any(pu => pu.Id == p.Id)).ToList();
        }

        private async Task<List<WorkspaceDTO>> GetListWorkspaceContainsUser(string userId)
        {
            var workspaces = await GetWorkspaces();
            var workspaceHaveUser = await GetListWorkspacesContainsUser(userId);
            return workspaces.Where(p => workspaceHaveUser.Any(pu => pu.Id == p.Id)).ToList();
        }

        private async Task<List<WorkspaceDTO>> GetWorkspaces()
        {
            var response = await _workspaceService.GetAllAsync<APIResponse>();
            List<WorkspaceDTO> list = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                list = JsonConvert.DeserializeObject<List<WorkspaceDTO>>(Convert.ToString(response.Result));
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
                foreach(var item in list)
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
                foreach(var item in list) 
                {
                    projects.Add(await GetProjectByIdAsync(item.ProjectId));
                }
            }
            return projects;
        }


        private async Task<List<WorkspaceDTO>> GetListWorkspacesContainsUser(string userId)
        {
            var response = await _workspaceUserService.GetByUserIdAsync<APIResponse>(userId);
            List<WorkspaceDTO> workspaces = new();
            List<WorkspaceUserDTO> list = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                list = JsonConvert.DeserializeObject<List<WorkspaceUserDTO>>(Convert.ToString(response.Result));
                foreach (var item in list)
                {
                    workspaces.Add(await GetWorkspaceByIdAsync(item.WorkspaceId));
                }
            }
            return workspaces;
        }
        private async Task<WorkspaceDTO> GetWorkspaceByIdAsync(int id)
        {
            var response = await _workspaceService.GetAsync<APIResponse>(id);
            WorkspaceDTO obj = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                obj = JsonConvert.DeserializeObject<WorkspaceDTO>(Convert.ToString(response.Result));
            }

            if (obj.Id != 0)
            {

                var resWorkspaceUsers = await _workspaceUserService.GetByWorkspaceIdAsync<APIResponse>(obj.Id);
                if (resWorkspaceUsers != null && resWorkspaceUsers.IsSuccess && resWorkspaceUsers.ErrorMessages.Count == 0)
                {
                    obj.WorkspaceUsers = JsonConvert.DeserializeObject<List<WorkspaceUserDTO>>(Convert.ToString(resWorkspaceUsers.Result));
                    obj.WorkspaceUserIds = obj.WorkspaceUsers.Select(x => x.UserId).ToList();
                }
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
    }
}