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
        private readonly ITaskMediaService _taskMediaService;

        public TaskController(IStatusService statusService,
            IWorkspaceService workspaceService, IUserService userService,
            IPriorityService priorityService, IColorService colorService, 
            ITaskUserService taskUserService, IProjectService projectService, 
            ITaskService taskService, IProjectUserService projectUserService, 
            ITaskMediaService taskMediaService ) : base(workspaceService)
        {
            _taskMediaService = taskMediaService;
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


        public async Task<IActionResult> IndexAsync(int? id)
        {
            if(id == null || id == 0)
            {
                TempData["warning"] = "Please choose project!";
                return RedirectToAction("Index", "Project");
            }
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("AccessDenied", "Auth");
            if (ViewBag.selectedWorkspaceId == null || ViewBag.selectedWorkspaceId == 0)
            {
                TempData["error"] = "You don't have any workspaces! Please create a workspace first!";
                return RedirectToAction("Create", "Workspace");
            }
            List<StatusDTO> statuses = await GetTaskByProjectIdAndUserIdAsync(userId, (int)id);
            ViewBag.statuses = await GetStatusesByUserIdAsync(userId); 

            return View(statuses);
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("AccessDenied", "Auth");
            TaskDTO obj = await GetTaskByIdAsync(id);

            var project = await GetProjectByIdAsync(obj.ProjectId);
            if (project.ActualEndAt != null && !string.IsNullOrEmpty(project.ActualEndAt.ToString()))
            {
                TempData["error"] = "Cannot update task for project end";
                return RedirectToAction("Index", "Task", new { id = project.Id });
            }
            if (obj == null)
            {
                TempData["error"] = "Task not found!";
                return RedirectToAction("Index", "Project");
            }

            List<UserDTO> users = await GetUsersByProjectIdAsync(obj.ProjectId);
            users.Add(await GetUserByIdAsync(userId));
            List<StatusDTO> statuses = await GetStatusesByUserIdAsync(userId);

            ViewBag.users = users;
            ViewBag.statuses = statuses;
            return View(obj);
        }

        public async Task<IActionResult> CreateAsync(int? id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("AccessDenied", "Auth");
            if (ViewBag.selectedWorkspaceId == null || ViewBag.selectedWorkspaceId == 0)
            {
                TempData["error"] = "You don't have any workspaces! Please create a workspace first!";
                return RedirectToAction("Create", "Workspace");
            }
            TaskDTO task = new();
            ViewBag.statuses = await GetStatusesByUserIdAsync(userId);
            if (id == null)
            {
                ViewBag.projects = await GetProjectByUserIdAndWorkspaceIdAsync(userId, ViewBag.selectedWorkspaceId);
            }
            else
            {
                
                var project = await GetProjectByIdAsync((int)id);
                if (project.ActualEndAt != null && !string.IsNullOrEmpty(project.ActualEndAt.ToString()))
                {
                    TempData["error"] = "Cannot create task for project end";
                    return RedirectToAction("Index", "Task", new { id = project.Id });
                }
                ViewBag.projects = new List<ProjectDTO>() { project };
                task.ProjectId = (int)id;
            }

            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(TaskDTO obj)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(userId) || !obj.OwnerId.Equals(userId))
                {
                    return RedirectToAction("AccessDenied", "Auth");
                }
                if (ViewBag.selectedWorkspaceId == null || ViewBag.selectedWorkspaceId == 0)
                {
                    TempData["error"] = "You don't have any workspaces! Please create a workspace first!";
                    return RedirectToAction("Create", "Workspace");
                }
                obj.Id = 0;
                obj.CreatedDate = DateTime.Now;
                APIResponse result = await _taskService.CreateAsync<APIResponse>(obj);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                {
                    var model = JsonConvert.DeserializeObject<TaskDTO>(Convert.ToString(result.Result));
                    if (obj.TaskUserIds != null && obj.TaskUserIds.Count > 0)
                        if(await AddUserToTask(obj.TaskUserIds, model.Id))
                            TempData["success"] = "Create new task successfully";

                    return RedirectToAction("Index", "Task", new {id = obj.ProjectId});
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
            return RedirectToAction("Index", "Task", new { id = obj.ProjectId });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatusAsync(int id, int status)
        {
            if (id == 0)
            {
                return Json(new { error = true, message = "Invalid task ID" });
            }

            var response = await _taskService.GetAsync<APIResponse>(id);
            TaskDTO obj = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                obj = JsonConvert.DeserializeObject<TaskDTO>(Convert.ToString(response.Result));
                if(obj != null)
                {
                    var project = await GetProjectByIdAsync(obj.ProjectId);
                    if (project.ActualEndAt != null && !string.IsNullOrEmpty(project.ActualEndAt.ToString()))
                    {
                        return Json(new { error = true, message = "Cannot update status task for project end" });
                    }
                }
            }
            else
            {
                return Json(new { error = true, message = "Task not found" });
            }

            obj.StatusId = status;
            obj.UpdatedDate = DateTime.Now;
            var res = await _taskService.UpdateAsync<APIResponse>(obj);

            if (res != null && res.IsSuccess)
            {
                return Json(new { error = false, message = "Task status updated successfully!" });
            }
            else
            {
                return Json(new { error = true, message = $"Failed to update task status! {res.ErrorMessages.FirstOrDefault()}" });
            }
        }
        public async Task<IActionResult> GetUsersByProject(int id)
        {
            var resUser = await _projectUserService.GetAsync<APIResponse>(id);
            var users = new List<ProjectUserDTO>();
            if (resUser != null && resUser.IsSuccess && resUser.ErrorMessages.Count == 0)
            {
                users = JsonConvert.DeserializeObject<List<ProjectUserDTO>>(Convert.ToString(resUser.Result));
                foreach (var item in users)
                {
                    item.User = await GetUserByIdAsync(item.UserId);
                }
            }
            var userList = users.Select(u => new { id = u.UserId, name = $"{u.User?.FirstName} {u.User?.LastName}" }).ToList();

            return Json(new { users = userList });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAsync(TaskDTO obj)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(userId) || !obj.OwnerId.Equals(userId))
                {
                    return RedirectToAction("AccessDenied", "Auth");
                }
                if (ViewBag.selectedWorkspaceId == null || ViewBag.selectedWorkspaceId == 0)
                {
                    TempData["error"] = "Internal server error!";
                    return RedirectToAction("Dashboard", "Home");
                }
                var project = await GetProjectByIdAsync(obj.ProjectId);
                if (project.ActualEndAt != null && !string.IsNullOrEmpty(project.ActualEndAt.ToString()))
                {
                    TempData["error"] = "Cannot create task for project end";
                    
                    return RedirectToAction("Index", "Task", new {id = project.Id});
                }
                obj.UpdatedDate = DateTime.Now;
                APIResponse result = await _taskService.UpdateAsync<APIResponse>(obj);
                TaskDTO existingTask = await GetTaskByIdAsync(obj.Id);
                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                {
                    var task = JsonConvert.DeserializeObject<TaskDTO>(Convert.ToString(result.Result));
                    if (obj.TaskUserIds != null && obj.TaskUserIds.Count > 0)
                        await UpdateTaskUsers(obj.TaskUserIds, existingTask.TaskUsers.Select(x => x.UserId).ToList(), obj.Id);
                    
                    TempData["success"] = "Update task successfully";
                    return RedirectToAction("Update", "Task", new { id = obj.Id });
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

            List<UserDTO> users = await GetUsersByProjectIdAsync(obj.ProjectId);
            users.Add(await GetUserByIdAsync(userId));
            List<StatusDTO> statuses = await GetStatusesByUserIdAsync(userId);

            ViewBag.users = users;
            ViewBag.statuses = statuses;
            return View(obj);
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
                List<TaskMediaDTO> medias = new List<TaskMediaDTO>();
                APIResponse resultTaskMedia = await _taskMediaService.GetByTaskIdAsync<APIResponse>(id);
                if (resultTaskMedia != null && resultTaskMedia.IsSuccess && resultTaskMedia.ErrorMessages.Count == 0)
                {
                    medias = JsonConvert.DeserializeObject<List<TaskMediaDTO>>(Convert.ToString(resultTaskMedia.Result));
                    if (medias != null && medias.Count > 0)
                    {
                        foreach (var item in medias)
                        {
                            APIResponse otherResult = await _taskMediaService.DeleteAsync<APIResponse>(item.Id);
                            if (otherResult == null && !otherResult.IsSuccess && otherResult.ErrorMessages.Count != 0)
                                return Json(new { error = true, message = otherResult?.ErrorMessages?.FirstOrDefault() ?? "An error occurred while deleting the task" });
                        }
                    }
                }

                List<TaskUserDTO> userDTOs = new List<TaskUserDTO>();
                APIResponse resultTaskUsers = await _taskUserService.GetByTaskIdAsync<APIResponse>(id);
                if (resultTaskUsers != null && resultTaskUsers.IsSuccess && resultTaskUsers.ErrorMessages.Count == 0)
                {
                    userDTOs = JsonConvert.DeserializeObject<List<TaskUserDTO>>(Convert.ToString(resultTaskUsers.Result));
                    if(userDTOs != null && userDTOs.Count > 0)
                    {
                        foreach (var item in userDTOs)
                        {
                            APIResponse otherResultUser = await _taskUserService.DeleteAsync<APIResponse>(item.Id);
                            if (otherResultUser == null && !otherResultUser.IsSuccess && otherResultUser.ErrorMessages.Count != 0)
                                return Json(new { error = true, message = otherResultUser?.ErrorMessages?.FirstOrDefault() ?? "An error occurred while deleting the task" });
                        }
                    }
                }

                APIResponse result = await _taskService.DeleteAsync<APIResponse>(id);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                    return Json(new { error = false, message = "Task deleted successfully" });
                else
                    return Json(new { error = true, message = result?.ErrorMessages?.FirstOrDefault() ?? "An error occurred while deleting the task" });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, message = "An error occurred: " + ex.Message });
            }
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

        private async Task<List<StatusDTO>> GetStatusesByUserIdAsync(string userId)
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
            }

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
                list = list.Where(x => x.ActualEndAt == null && !string.IsNullOrEmpty(x.ActualEndAt.ToString())).ToList();
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
        private async Task<bool> AddUserToTask(List<string> userIds, int id)
        {
            try
            {
                foreach (var user in userIds)
                {
                    var obj = new TaskUserDTO { UserId = user, TaskId = id };
                    var response = await _taskUserService.CreateAsync<APIResponse>(obj);
                    if (response == null && response.IsSuccess && response.ErrorMessages.Count == 0)
                    {
                        TempData["error"] = response.ErrorMessages.FirstOrDefault();
                        return false;
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Internal Server Error! {ex.Message}";
                return false;
            }
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
                    var resUser = await _taskUserService.GetByTaskIdAsync<APIResponse>(item.Id);
                    if (resUser != null && resUser.IsSuccess && resUser.ErrorMessages.Count == 0)
                    {
                        users = JsonConvert.DeserializeObject<List<TaskUserDTO>>(Convert.ToString(resUser.Result));
                        item.TaskUsers = users;
                    }
                }
            }
            return list;
        }


        private async Task<TaskDTO> GetTaskByIdAsync(int id)
        {
            var response = await _taskService.GetAsync<APIResponse>(id);
            TaskDTO obj = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                obj = JsonConvert.DeserializeObject<TaskDTO>(Convert.ToString(response.Result));
                obj.Project = await GetProjectByIdAsync(obj.ProjectId);
            }
            return obj;
        }

        private async Task<List<UserDTO>> GetUsersByProjectIdAsync(int projectId)
        {
            var response = await _projectUserService.GetByProjectIdAsync<APIResponse>(projectId);
            List<ProjectUserDTO> list = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                list = JsonConvert.DeserializeObject<List<ProjectUserDTO>>(Convert.ToString(response.Result));
                foreach(var item in list) 
                {
                    item.User = await GetUserByIdAsync(item.UserId);
                }
            }
            var users = new List<UserDTO>(); 
            if (list.Count > 0)
            {
                users = list.Select(x => x.User).ToList();
            }
            return users;
        }


        private async Task<ProjectDTO> GetProjectByIdAsync(int id)
        {
            var response = await _projectService.GetAsync<APIResponse>(id);
            ProjectDTO obj = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                obj = JsonConvert.DeserializeObject<ProjectDTO>(Convert.ToString(response.Result));
            }
            return obj;
        }

        private async Task<bool> UpdateTaskUsers(List<string> userIdsNew, List<string> userIdsOld, int taskId)
        {
            try
            {

                var usersToAdd = userIdsNew.Except(userIdsOld).ToList();
                var usersToRemove = userIdsOld.Except(userIdsNew).ToList();

                foreach (var userId in usersToAdd)
                {
                    var response = await _taskUserService.CreateAsync<APIResponse>(new TaskUserDTO { TaskId = taskId, UserId = userId });
                    if (response == null && response.IsSuccess && response.ErrorMessages.Count == 0)
                    {
                        TempData["error"] = response.ErrorMessages.FirstOrDefault();
                        return false;
                    }

                }
                foreach (var userId in usersToRemove)
                {
                    var response = await _taskUserService.DeleteByTaskAndUserAsync<APIResponse>(taskId, userId);
                    if (response == null && response.IsSuccess && response.ErrorMessages.Count == 0)
                    {
                        TempData["error"] = response.ErrorMessages.FirstOrDefault();
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Internal Server Error! {ex.Message}";
                return false;
            }
        }
    }
}
