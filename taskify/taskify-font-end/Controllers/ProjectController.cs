using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
using taskify_font_end.Service.IService;

namespace taskify_font_end.Controllers
{
    public class ProjectController : BaseController
    {

        private readonly IProjectService _projectService;
        private readonly IWorkspaceService _workspaceService;
        private readonly IUserService _userService;
        private readonly IStatusService _statusService;
        private readonly ITagService _tagService;
        private readonly IProjectUserService _projectUserService;
        private readonly IProjectTagService _projectTagService;
        private readonly IMapper _mapper;
        private readonly int ITEM_PER_PAGE = 0;
        public ProjectController(IProjectService projectService, IMapper mapper,
            IWorkspaceService workspaceService, IUserService userService,
            IStatusService statusService, ITagService tagService,
            IProjectUserService projectUserService, IProjectTagService projectTagService, IConfiguration configuration) : base(workspaceService)
        {
            _workspaceService = workspaceService;
            _mapper = mapper;
            _projectService = projectService;
            _userService = userService;
            _statusService = statusService;
            _tagService = tagService;
            _projectUserService = projectUserService;
            _projectTagService = projectTagService;
            ITEM_PER_PAGE = configuration.GetValue<int>("ItemPerPage");
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
            List<UserDTO> users = await GetUsersAsync(userId);
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

                if (result != null && result.IsSuccess)
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
            List<UserDTO> users = await GetUsersAsync(userId);
            List<StatusDTO> statuses = await GetStatusesByUserIdAsync(userId);
            List<TagDTO> tags = await GetTagsByUserIdAsync(userId);

            ViewBag.users = users;
            ViewBag.tags = tags;
            ViewBag.statuses = statuses;
            return View(obj);
        }

        public IActionResult Update(int id)
        {
            return View();
        }

        public IActionResult Detail(int id)
        {
            return View();
        }

        public IActionResult Delete(int id)
        {
            return View();
        }

        private async Task<bool> AddTagToProject(List<int> tagIds, int projectId)
        {
            try
            {
                foreach (var item in tagIds)
                {
                    var projectTag = new ProjectTagDTO { TagId = item, ProjectId = projectId };
                    var response = await _projectTagService.CreateAsync<APIResponse>(projectTag);
                    if (response == null && response.IsSuccess)
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
                    if (response == null && response.IsSuccess)
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


        private async Task<List<TagDTO>> GetTagsByUserIdAsync(string userId)
        {
            var response = await _tagService.GetByUserIdAsync<APIResponse>(userId);
            List<TagDTO> list = new();
            if (response != null && response.IsSuccess)
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
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<StatusDTO>>(Convert.ToString(response.Result));
            }

            if (list.Count == 0)
                TempData["warning"] = "No status are available.";

            return list;
        }

        private async Task<List<UserDTO>> GetUsersAsync(string userId)
        {
            var response = await _userService.GetAllAsync<APIResponse>();
            List<UserDTO> users = new();
            if (response != null && response.IsSuccess)
            {
                users = JsonConvert.DeserializeObject<List<UserDTO>>(Convert.ToString(response.Result));
            }
            if (users.Count > 0) users = users.Where(x => !x.Id.Equals(userId)).ToList();


            if (users.Count == 0)
                TempData["warning"] = "No users are available.";

            return users;
        }

        private async Task<List<ProjectDTO>> GetProjectByUserIdAndWorkspaceIdAsync(string userId, int workspaceId)
        {
            var response = await _projectService.GetByUserIdAndWorkspaceIdAsync<APIResponse>(userId, workspaceId);
            List<ProjectDTO> list = new();
            List<ProjectTagDTO> tags = new();
            if (response != null && response.IsSuccess)
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

                    if (res != null && res.IsSuccess)
                    {
                        tags = JsonConvert.DeserializeObject<List<ProjectTagDTO>>(Convert.ToString(res.Result));
                        item.ProjectTags = tags;
                    }
                }
            }
            return list;
        }

    }
}
