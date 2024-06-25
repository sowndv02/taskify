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
        private readonly IUserService _userService;
        private readonly IStatusService _statusService; 
        private readonly ITagService _tagService; 
        private readonly IMapper _mapper;
        public ProjectController(IProjectService projectService, IMapper mapper, IWorkspaceService workspaceService, IUserService userService, IStatusService statusService, ITagService tagService) : base(workspaceService)
        {
            _workspaceService = workspaceService;
            _mapper = mapper;
            _projectService = projectService;
            _userService = userService;
            _statusService = statusService;
            _tagService = tagService;

        }
        public IActionResult Index(int? page, string? sort, int? status)
        {

            return View();
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

    }
}
