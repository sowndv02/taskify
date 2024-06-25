using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
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

        public async Task<IActionResult> Index()
        {
            
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("AccessDenied", "Auth");

            List<WorkspaceDTO> list = await GetWorkspaceByUserIdAsync(userId);

            return View(list);
        }

        public async Task<IActionResult> CreateAsync()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            List<UserDTO> users = await GetUsersAsync(userId);
            ViewBag.users = users;
            WorkspaceDTO workspaceDTO = new WorkspaceDTO();
            return View(workspaceDTO);
        }

        public async Task<IActionResult> RemoveMe(int id)
        {
            int newWorkspaceId = await GetNewIdWorkspace();
            if (id == 0)
            {
                return RedirectToAction("Dashboard", "Home", new { id = newWorkspaceId });
            }
            var response = await _workspaceService.GetAsync<APIResponse>(id);
            WorkspaceDTO workspace = new();
            if(response != null && response.IsSuccess)
            {
                workspace = JsonConvert.DeserializeObject<WorkspaceDTO>(Convert.ToString(response.Result));
            }
            workspace.IsDeleted = true;
            var res = await _workspaceService.UpdateAsync<APIResponse>(workspace);
            if(res != null && res.IsSuccess)
            {
                TempData["success"] = "You have successfully left the workspace!";
                return RedirectToAction("Dashboard", "Home", new { id = newWorkspaceId });
            }
            else
            {
                TempData["error"] = $"You have failed to leave the workspace! {res.ErrorMessages.FirstOrDefault()}";
                return RedirectToAction("Dashboard", "Home", new { id });
            }
        }

        private async Task<int> GetNewIdWorkspace()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var list = await GetWorkspaceByUserIdAsync(userId);
            if (list.Count > 0)
                return (int)(list.FirstOrDefault()?.Id);
            else return 0;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(WorkspaceDTO obj)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (ModelState.IsValid)
            {

                if (string.IsNullOrEmpty(userId) || !obj.OwnerId.Equals(userId))
                {
                    return RedirectToAction("AccessDenied", "Auth");
                }
                APIResponse result = await _workspaceService.CreateAsync<APIResponse>(obj);

                if (result != null && result.IsSuccess)
                {
                    var workspace = JsonConvert.DeserializeObject<WorkspaceDTO>(Convert.ToString(result.Result));
                    await AddUserToWorkSpace(obj.WorkspaceUserIds, workspace.Id);
                    TempData["success"] = "Create new workspace successfully";
                    List<WorkspaceDTO> workspaces = new List<WorkspaceDTO>();
                    if (!string.IsNullOrEmpty(userId))
                    {
                        workspaces = await GetWorkspaceByUserIdAsync(userId);
                        ViewBag.workspaces = workspaces;
                    }
                    return RedirectToAction("Index", "Workspace");
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
            ViewBag.users = users;
            return View(obj);
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

        private async Task<bool> AddUserToWorkSpace(List<string> userIds, int workspaceId)
        {
            try
            {
                foreach (var user in userIds)
                {
                    var workspaceUser = new WorkspaceUserDTO { UserId = user, WorkspaceId = workspaceId };
                    var response = await _workspaceUserService.CreateAsync<APIResponse>(workspaceUser);
                    if (response == null && !response.IsSuccess)
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


        private async Task<List<WorkspaceDTO>> GetWorkspaceByUserIdAsync(string userId)
        {
            var response = await _workspaceService.GetByUserIdAsync<APIResponse>(userId);
            List<WorkspaceDTO> workspaces = new();
            if (response != null && response.IsSuccess)
            {
                workspaces = JsonConvert.DeserializeObject<List<WorkspaceDTO>>(Convert.ToString(response.Result));
            }
            if (workspaces.Count > 0)
            {
                workspaces = workspaces.OrderByDescending(x => x.CreatedDate)
                   .ThenByDescending(x => x.UpdatedDate)
                   .ToList();
            }
            return workspaces;
        }

    }
}
