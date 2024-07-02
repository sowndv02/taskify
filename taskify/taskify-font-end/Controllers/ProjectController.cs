using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
using taskify_font_end.Service;
using taskify_font_end.Service.IService;

namespace taskify_font_end.Controllers
{
    public class ProjectController : BaseController
    {

        private readonly IProjectService _projectService;
        private readonly IWorkspaceService _workspaceService;
        private readonly IWorkspaceUserService _workspaceUserService;
        private readonly IUserService _userService;
        private readonly IStatusService _statusService;
        private readonly ITagService _tagService;
        private readonly IProjectUserService _projectUserService;
        private readonly IProjectTagService _projectTagService;
        private readonly ITaskService _taskService;
        private readonly ITaskUserService _taskUserService;
        private readonly IMilestoneService _milestoneService;
        private readonly IProjectMediaService _projectMediaService;
        private readonly IMapper _mapper;
        private readonly int ITEM_PER_PAGE = 0;

        public ProjectController(IProjectService projectService, IMapper mapper,
            IWorkspaceService workspaceService, IUserService userService,
            IStatusService statusService, ITagService tagService,
            IProjectUserService projectUserService, IProjectTagService projectTagService, 
            ITaskService taskService, ITaskUserService taskUserService, 
            IWorkspaceUserService workspaceUserService,
            IMilestoneService milestoneService,
            IProjectMediaService projectMediaService,
            IConfiguration configuration) : base(workspaceService)
        {
            _workspaceUserService = workspaceUserService;
            _workspaceService = workspaceService;
            _mapper = mapper;
            _projectService = projectService;
            _userService = userService;
            _statusService = statusService;
            _tagService = tagService;
            _taskService = taskService;
            _projectUserService = projectUserService;
            _projectTagService = projectTagService;
            ITEM_PER_PAGE = configuration.GetValue<int>("ItemPerPage");
            _taskUserService = taskUserService;
            _milestoneService = milestoneService;   
            _projectMediaService = projectMediaService;
        }
        public async Task<IActionResult> IndexAsync(int? page, string? sort, int? status, int[]? tagIds)
        {

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("AccessDenied", "Auth");
            if (ViewBag.selectedWorkspaceId == null || ViewBag.selectedWorkspaceId == 0)
            {
                TempData["error"] = "You don't have any workspaces! Please create a workspace first!";
                return RedirectToAction("Create", "Workspace");
            }
            List<ProjectDTO> list = await GetProjectByUserIdAndWorkspaceIdAsync(userId, ViewBag.selectedWorkspaceId);

            if (status != null)
            {
                list = list.Where(x => x.StatusId == status).ToList();
                ViewBag.Status = status;
            }
            if (sort != null && !string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower())
                {
                    case "newest":
                        list = list.OrderByDescending(x => x.CreatedDate).ToList();
                        break;
                    case "oldest":
                        list = list.OrderBy(x => x.CreatedDate).ToList();
                        break;
                    case "recently-updated":
                        list = list.OrderByDescending(x => x.UpdatedDate).ToList();
                        break;
                    case "earliest-updated":
                        list = list.OrderBy(x => x.UpdatedDate).ToList();
                        break;
                    default:
                        list = list.OrderByDescending(x => x.CreatedDate).ToList();
                        break;
                }
                ViewBag.sort = sort;
            }
            if (tagIds != null && tagIds.Length > 0)
            {
                list = list.Where(p => p.ProjectTags.Any(pt => tagIds.Contains(pt.TagId))).ToList();
                ViewBag.tagIds = tagIds;
            }
            if (list.Count > 0) ViewBag.total = list.Count;
            if (page != null && ITEM_PER_PAGE > 0 && list.Count > 0)
            {
                int totalPage = ((int)list.Count / 6) + 1;
                if (page <= 0) page = 1;
                if (page > totalPage) page = totalPage;
                ViewBag.page = page;
                ViewBag.totalPage = totalPage;
                list = list.Skip((int)(page - 1) * ITEM_PER_PAGE).Take(ITEM_PER_PAGE).ToList();
            }
            else if (list.Count > 0 && ITEM_PER_PAGE > 0)
            {
                int totalPage = ((int)list.Count / 6) + 1;
                list = list.Take(ITEM_PER_PAGE).ToList();
                ViewBag.page = 1;
                ViewBag.totalPage = totalPage;
            }
            else
            {
                ViewBag.page = 1;
                ViewBag.totalPage = 0;
            }
            if (ITEM_PER_PAGE > 0) ViewBag.perPage = ITEM_PER_PAGE;
            ViewBag.tags = await GetTagsByUserIdAsync(userId);
            ViewBag.statuses = await GetStatusesByUserIdAsync(userId);
            return View(list);
        }

        public async Task<IActionResult> CreateAsync()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            List<UserDTO> users = await GetUsersByWorkspaceIdAsync(userId, ViewBag.selectedWorkspaceId);
            List<StatusDTO> statuses = await GetStatusesByUserIdAsync(userId);
            List<TagDTO> tags = await GetTagsByUserIdAsync(userId);

            ViewBag.users = users;
            ViewBag.tags = tags;
            ViewBag.statuses = statuses;
            ProjectDTO obj = new ProjectDTO();
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(ProjectDTO obj)
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
                obj.WorkspaceId = ViewBag.selectedWorkspaceId;

                APIResponse result = await _projectService.CreateAsync<APIResponse>(obj);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                {
                    var project = JsonConvert.DeserializeObject<ProjectDTO>(Convert.ToString(result.Result));
                    if (obj.ProjectUserIds != null && obj.ProjectUserIds.Count > 0)
                        await AddUserToProject(obj.ProjectUserIds, project.Id);
                    if (obj.ProjectTagIds != null && obj.ProjectTagIds.Count > 0)
                        await AddTagToProject(obj.ProjectTagIds, project.Id);
                    TempData["success"] = "Create new project successfully";
                    return RedirectToAction("Index", "Project");
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
            List<UserDTO> users = await GetUsersByWorkspaceIdAsync(userId, ViewBag.selectedWorkspaceId);
            List<StatusDTO> statuses = await GetStatusesByUserIdAsync(userId);
            List<TagDTO> tags = await GetTagsByUserIdAsync(userId);

            ViewBag.users = users;
            ViewBag.tags = tags;
            ViewBag.statuses = statuses;
            return View(obj);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateAsync(int id)
        {
            if(id <= 0) return BadRequest();
            
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("AccessDenied", "Auth");

            List<UserDTO> users = await GetUsersByWorkspaceIdAsync(userId, ViewBag.selectedWorkspaceId);
            List<StatusDTO> statuses = await GetStatusesByUserIdAsync(userId);
            List<TagDTO> tags = await GetTagsByUserIdAsync(userId);

            ViewBag.users = users;
            ViewBag.tags = tags;
            ViewBag.statuses = statuses;
            ProjectDTO obj = await GetProjectByIdAsync(id);
            if(obj.ActualEndAt != null || !string.IsNullOrEmpty(obj.ActualEndAt.ToString())) RedirectToAction("AccessDenied", "Auth");
            return View(obj);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAsync(ProjectDTO obj)
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
                obj.WorkspaceId = ViewBag.selectedWorkspaceId;

                APIResponse result = await _projectService.UpdateAsync<APIResponse>(obj);
                ProjectDTO existingProject = await GetProjectByIdAsync(obj.Id);
                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                {
                    var project = JsonConvert.DeserializeObject<ProjectDTO>(Convert.ToString(result.Result));
                    if (obj.ProjectUserIds != null && obj.ProjectUserIds.Count > 0)
                        await UpdateProjectUsers(obj.ProjectUserIds, existingProject.ProjectUsers.Select(x => x.UserId).ToList(), obj.Id);
                    if (obj.ProjectTagIds != null && obj.ProjectTagIds.Count > 0)
                        await UpdateProjectTags(obj.ProjectTagIds, existingProject.ProjectTags.Select(x => x.TagId).ToList(), obj.Id);
                    TempData["success"] = "Update project successfully";
                    return RedirectToAction("Update", "Project", new {id = obj.Id});
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
            List<UserDTO> users = await GetUsersByWorkspaceIdAsync(userId, ViewBag.selectedWorkspaceId);
            List<StatusDTO> statuses = await GetStatusesByUserIdAsync(userId);
            List<TagDTO> tags = await GetTagsByUserIdAsync(userId);

            ViewBag.users = users;
            ViewBag.tags = tags;
            ViewBag.statuses = statuses;
            return View(obj);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            ProjectDTO project = await GetProjectByIdAsync(id);
            if (string.IsNullOrEmpty(userId) || !project.OwnerId.Equals(userId))
            {
                return RedirectToAction("AccessDenied", "Auth");
            }
            if (ViewBag.selectedWorkspaceId == null || ViewBag.selectedWorkspaceId == 0)
            {
                TempData["error"] = "Internal server error!";
                return RedirectToAction("Dashboard", "Home");
            }
            ViewBag.tasks = await GetTaskByProjectId(id);
            ViewBag.statuses = await GetTaskByProjectIdAndUserIdAsync(userId, id);
            ViewBag.milestones = await GetMilestoneByProjectId(id);
            ViewBag.projectMedias = await GetProjectMediaByProjectId(id);
            return View(project);
        }

        public async Task<IActionResult> DuplicateAsync(int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("AccessDenied", "Auth");
            }
            var obj = await GetProjectByIdAsync(id);
            if (obj == null)
            {
                return Json(new { error = true, message = "Project not found"});
            }
            if (!obj.OwnerId.Equals(userId))
            {
                return Json(new { error = true, message = "Access denied"});
            }
            try
            {
                obj.ActualEndAt = null;
                obj.Id = 0;
                obj.ProjectTags = new List<ProjectTagDTO>();
                obj.ProjectUsers = new List<ProjectUserDTO>();
                var duplicatedProject = obj;

                APIResponse result = await _projectService.CreateAsync<APIResponse>(duplicatedProject);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                {
                    var project = JsonConvert.DeserializeObject<ProjectDTO>(Convert.ToString(result.Result));
                    if (obj.ProjectUserIds != null && obj.ProjectUserIds.Count > 0)
                        await AddUserToProject(obj.ProjectUserIds, project.Id);
                    if (obj.ProjectTagIds != null && obj.ProjectTagIds.Count > 0)
                        await AddTagToProject(obj.ProjectTagIds, project.Id);
                    return Json(new { error = false, message = "Project duplicated successfully", duplicatedProject});
                }
                else
                {
                    return Json(new { error = false, message = result.ErrorMessages.FirstOrDefault()});
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = true, message = ex.Message});
            }
        }

        public async Task<IActionResult> FinishAsync(int id)
        {
            if (id == 0) return RedirectToAction("Index", "Project");
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("AccessDenied", "Auth");
            ProjectDTO obj = await GetProjectByIdAsync(id);
            if (!obj.OwnerId.Equals(userId)) return RedirectToAction("AccessDenied", "Auth");
            obj.ProjectTags = new List<ProjectTagDTO>();
            obj.ProjectUsers = new List<ProjectUserDTO>();
            obj.ActualEndAt = DateTime.Now;
            try
            {
                APIResponse result = await _projectService.UpdateAsync<APIResponse>(obj);
                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                {
                    var project = JsonConvert.DeserializeObject<ProjectDTO>(Convert.ToString(result.Result));
                    TempData["success"] = "Finish project successfully";
                    return RedirectToAction("Index", "Project");
                }
                else
                {
                    TempData["error"] = result.ErrorMessages.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message.ToString();
                return RedirectToAction("Index", "Project");
            }

            return RedirectToAction("Index", "Project");
        }


        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                return Json(new { error = true, message = "Invalid ID" });
            }
            try
            {
                bool resultDeleteTask = await DeleteTaskByProjectId(id);
                if(!resultDeleteTask) return Json(new { error = true, message = "Internal server have error!" });

                bool resultDeleteProjectUsers = await DeleteProjectUsersByProjectId(id);
                if (!resultDeleteProjectUsers) return Json(new { error = true, message = "Internal server have error!" });
                
                bool resultDeleteProjectTags = await DeleteProjectTagsByProjectId(id);
                if (!resultDeleteProjectTags) return Json(new { error = true, message = "Internal server have error!" });

                APIResponse result = await _projectService.DeleteAsync<APIResponse>(id);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                    return Json(new { error = false, message = "Project deleted successfully" });
                else
                    return Json(new { error = true, message = result?.ErrorMessages?.FirstOrDefault() ?? "An error occurred while deleting the todo" });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, message = "An error occurred: " + ex.Message });
            }
        }

        private async Task<bool> AddTagToProject(List<int> tagIds, int projectId)
        {
            try
            {
                foreach (var item in tagIds)
                {
                    var projectTag = new ProjectTagDTO { TagId = item, ProjectId = projectId };
                    var response = await _projectTagService.CreateAsync<APIResponse>(projectTag);
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

        private async Task<bool> AddUserToProject(List<string> userIds, int projectId)
        {
            try
            {
                foreach (var user in userIds)
                {
                    var projectUser = new ProjectUserDTO { UserId = user, ProjectId = projectId };
                    var response = await _projectUserService.CreateAsync<APIResponse>(projectUser);
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

        private async Task<ProjectDTO> GetProjectByIdAsync(int id)
        {
            var response = await _projectService.GetAsync<APIResponse>(id);
            ProjectDTO obj = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                obj = JsonConvert.DeserializeObject<ProjectDTO>(Convert.ToString(response.Result));
            }

            if(obj.Id != 0)
            {
                var res = await _projectTagService.GetAsync<APIResponse>(obj.Id);

                if (res != null && res.IsSuccess && res.ErrorMessages.Count == 0)
                {
                    obj.ProjectTags = JsonConvert.DeserializeObject<List<ProjectTagDTO>>(Convert.ToString(res.Result));
                    obj.ProjectTagIds = obj.ProjectTags.Select(x => x.TagId).ToList();
                }
                var resProjectUsers = await _projectUserService.GetAsync<APIResponse>(obj.Id);
                if (resProjectUsers != null && resProjectUsers.IsSuccess && resProjectUsers.ErrorMessages.Count == 0)
                {
                    obj.ProjectUsers = JsonConvert.DeserializeObject<List<ProjectUserDTO>>(Convert.ToString(resProjectUsers.Result));
                    obj.ProjectUserIds = obj.ProjectUsers.Select(x => x.UserId).ToList();
                }
            }

            return obj;
        }

        private async Task<List<TagDTO>> GetTagsByUserIdAsync(string userId)
        {
            var response = await _tagService.GetByUserIdAsync<APIResponse>(userId);
            List<TagDTO> list = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                list = JsonConvert.DeserializeObject<List<TagDTO>>(Convert.ToString(response.Result));
            }

            if (list.Count == 0)
                TempData["warning"] = "No tag are available.";

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

            if (list.Count == 0)
                TempData["warning"] = "No status are available.";

            return list;
        }

        private async Task<List<UserDTO>> GetUsersByWorkspaceIdAsync(string userId, int workspaceId)
        {
            var response = await _workspaceUserService.GetByWorkspaceIdAsync<APIResponse>(workspaceId);
            List<WorkspaceUserDTO> workspaceUsers = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                workspaceUsers = JsonConvert.DeserializeObject<List<WorkspaceUserDTO>>(Convert.ToString(response.Result));
            }
            var users = new List<UserDTO>();
            if (workspaceUsers.Count > 0)
            {
                users = workspaceUsers.Select(x => x.User).Where(x => !x.Id.Equals(userId)).ToList();
                
            }
            if (users.Count == 0)
                TempData["warning"] = "No users are available.";
            return users;
        }

        private async Task<List<ProjectDTO>> GetProjectByUserIdAndWorkspaceIdAsync(string userId, int workspaceId)
        {
            var response = await _projectService.GetByUserIdAndWorkspaceIdAsync<APIResponse>(userId, workspaceId);
            List<ProjectDTO> list = new();
            List<ProjectTagDTO> tags = new();
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
                    var res = await _projectTagService.GetAsync<APIResponse>(item.Id);

                    if (res != null && res.IsSuccess && res.ErrorMessages.Count == 0)
                    {
                        tags = JsonConvert.DeserializeObject<List<ProjectTagDTO>>(Convert.ToString(res.Result));
                        item.ProjectTags = tags;
                    }
                    var resUser = await _projectUserService.GetAsync<APIResponse>(item.Id);
                    if (resUser != null && resUser.IsSuccess && resUser.ErrorMessages.Count == 0)
                    {
                        users = JsonConvert.DeserializeObject<List<ProjectUserDTO>>(Convert.ToString(resUser.Result));
                        item.ProjectUsers = users;
                    }
                }
            }
            return list;
        }

        private async Task<bool> UpdateProjectUsers(List<string> userIdsNew, List<string> userIdsOld, int projectId)
        {
            try
            {

                var usersToAdd = userIdsNew.Except(userIdsOld).ToList();
                var usersToRemove = userIdsOld.Except(userIdsNew).ToList();

                foreach (var userId in usersToAdd)
                {
                    var response =  await _projectUserService.CreateAsync<APIResponse>(new ProjectUserDTO { ProjectId = projectId, UserId = userId });
                    if (response == null && response.IsSuccess && response.ErrorMessages.Count == 0)
                    {
                        TempData["error"] = response.ErrorMessages.FirstOrDefault();
                        return false;
                    }

                }
                foreach (var userId in usersToRemove)
                {
                    var response = await _projectUserService.DeleteByProjectAndUserAsync<APIResponse>(projectId, userId);
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

        private async Task<bool> UpdateProjectTags(List<int> tagIdsNew, List<int> tagIdsOld, int projectId)
        {
            try
            {

                var tagsToAdd = tagIdsNew.Except(tagIdsOld).ToList();
                var tagsToRemove = tagIdsOld.Except(tagIdsNew).ToList();

                foreach (var item in tagsToAdd)
                {
                    var response = await _projectTagService.CreateAsync<APIResponse>(new ProjectTagDTO { ProjectId = projectId, TagId = item });
                    if (response == null && response.IsSuccess && response.ErrorMessages.Count == 0)
                    {
                        TempData["error"] = response.ErrorMessages.FirstOrDefault();
                        return false;
                    }

                }
                foreach (var item in tagsToRemove)
                {
                    var response = await _projectTagService.DeleteByProjectAndTagAsync<APIResponse>(projectId, item);
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

        private async Task<bool> DeleteTaskByProjectId(int projectId)
        {
            var tasks = await GetTaskByProjectId(projectId);
            foreach (var task in tasks)
            {
                var taskUsers = task.TaskUsers;
                if(taskUsers != null && taskUsers.Count > 0)
                {
                    foreach(var item in taskUsers)
                    {
                        var result = await _taskUserService.DeleteAsync<APIResponse>(item.Id);
                        if (result == null || !result.IsSuccess || result.ErrorMessages.Count != 0)
                            return false;
                    }
                    
                }
                var taskResult = await _taskService.DeleteAsync<APIResponse>(task.Id);
                if (taskResult == null || !taskResult.IsSuccess || taskResult.ErrorMessages.Count != 0)
                    return false;
            }
            return true;
        }

        private async Task<List<TaskDTO>> GetTaskByProjectId(int id)
        {
            var response = await _taskService.GetByProjectIdAsync<APIResponse>(id);
            List<TaskDTO> list = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                list = JsonConvert.DeserializeObject<List<TaskDTO>>(Convert.ToString(response.Result));
            }
            return list;
        }


        private async Task<bool> DeleteProjectUsersByProjectId(int projectId)
        {
            var result = await _projectUserService.GetAsync<APIResponse>(projectId);
            var list = new List<ProjectUserDTO>();
            if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
            {
                list = JsonConvert.DeserializeObject<List<ProjectUserDTO>>(Convert.ToString(result.Result));
            }
            if(list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    var deleteResult = await _projectUserService.DeleteAsync<APIResponse>(item.Id);
                    if (deleteResult == null || !deleteResult.IsSuccess || deleteResult.ErrorMessages.Count != 0)
                        return false;
                }
            }
            
            return true;
        }

        private async Task<bool> DeleteProjectTagsByProjectId(int projectId)
        {
            var result = await _projectTagService.GetAsync<APIResponse>(projectId);
            var list = new List<ProjectTagDTO>();
            if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
            {
                list = JsonConvert.DeserializeObject<List<ProjectTagDTO>>(Convert.ToString(result.Result));
            }
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    var deleteResult = await _projectTagService.DeleteAsync<APIResponse>(item.Id);
                    if (deleteResult == null || !deleteResult.IsSuccess || deleteResult.ErrorMessages.Count != 0)
                        return false;
                }
            }

            return true;
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
        private async Task<List<MilestoneDTO>> GetMilestoneByProjectId(int id)
        {
            var response = await _milestoneService.GetByProjectIdAsync<APIResponse>(id);
            List<MilestoneDTO> list = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                list = JsonConvert.DeserializeObject<List<MilestoneDTO>>(Convert.ToString(response.Result));
            }
            return list;
        }

        private async Task<List<ProjectMediaDTO>> GetProjectMediaByProjectId(int id)
        {
            var response = await _projectMediaService.GetByProjectIdAsync<APIResponse>(id);
            List<ProjectMediaDTO> list = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                list = JsonConvert.DeserializeObject<List<ProjectMediaDTO>>(Convert.ToString(response.Result));
            }
            return list;
        }

    }
}
