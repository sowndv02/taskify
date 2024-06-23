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
    public class WorkspaceController : Controller
    {
        private readonly IUserService _userService;
        private readonly IWorkspaceService _workspaceService;
        private readonly IWorkspaceUserService _workspaceUserService;
        private readonly IMapper _mapper;
        public WorkspaceController(IUserService userService, IWorkspaceService workspaceService, IWorkspaceUserService workspaceUserService, IMapper mapper)
        {
            _mapper = mapper;
            _workspaceService = workspaceService;
            _workspaceUserService = workspaceUserService;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateAsync()
        {
            List<UserDTO> users = await GetUsersAsync();
            ViewBag.users = users;
            WorkspaceDTO workspaceDTO = new WorkspaceDTO();
            return View(workspaceDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(WorkspaceDTO obj)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userId) || !obj.OwnerId.Equals(userId) )
                {
                    return RedirectToAction("AccessDenied", "Auth");
                }
                APIResponse result = await _workspaceService.CreateAsync<APIResponse>(obj);
                if (result != null && result.IsSuccess)
                {
                    TempData["success"] = "Create new workspace successfully";
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
            List<UserDTO> users = await GetUsersAsync();
            ViewBag.users = users;
            return View(obj);
        }

        private async Task<List<UserDTO>> GetUsersAsync()
        {
            var response = await _userService.GetAllAsync<APIResponse>();
            List<UserDTO> users = new();
            if (response != null && response.IsSuccess)
            {
                users = JsonConvert.DeserializeObject<List<UserDTO>>(Convert.ToString(response.Result));
            }
            if(users.Count == 0)
            {
                TempData["warning"] = "No users are available.";
            }
            return users;
        }

        private async Task<UserDTO> GetUserByIdAsync(string userId)
        {
            var response = await _userService.GetAsync<APIResponse>(userId);
            UserDTO user = new();
            if (response != null && response.IsSuccess)
            {
                user = JsonConvert.DeserializeObject<UserDTO>(Convert.ToString(response.Result));
            }
            return user;
        }

    }
}
