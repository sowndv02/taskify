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
    public class WorkspaceController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IWorkspaceService _workspaceService;
        private readonly IWorkspaceUserService _workspaceUserService;
        private readonly IMapper _mapper;
        public WorkspaceController(
            IUserService userService, IWorkspaceService workspaceService, 
            IWorkspaceUserService workspaceUserService, IMapper mapper) : base(workspaceService, workspaceUserService)
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
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                workspace = JsonConvert.DeserializeObject<WorkspaceDTO>(Convert.ToString(response.Result));
            }
            workspace.IsDeleted = true;
            var res = await _workspaceService.UpdateAsync<APIResponse>(workspace);
            if (res != null && res.IsSuccess)
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

        [HttpGet]
        public async Task<IActionResult> UpdateAsync(int id)
        {
            if (id <= 0) return BadRequest();

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("AccessDenied", "Auth");

            List<UserDTO> users = await GetUsersAsync(userId);

            ViewBag.users = users;
            WorkspaceDTO obj = await GetWorkspaceByIdAsync(id);
            if (obj.IsDeleted) RedirectToAction("AccessDenied", "Auth");
            return View(obj);
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

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                {
                    var workspace = JsonConvert.DeserializeObject<WorkspaceDTO>(Convert.ToString(result.Result));
                    await AddUserToWorkSpace(obj.WorkspaceUserIds, workspace.Id);
                    TempData["success"] = "Create new workspace successfully";
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAsync(WorkspaceDTO obj)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(userId) || !obj.OwnerId.Equals(userId))
                {
                    return RedirectToAction("AccessDenied", "Auth");
                }
                
                APIResponse result = await _workspaceService.UpdateAsync<APIResponse>(obj);

                WorkspaceDTO existingObj = await GetWorkspaceByIdAsync(obj.Id);
                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                {
                    var workspace = JsonConvert.DeserializeObject<WorkspaceDTO>(Convert.ToString(result.Result));
                    if (obj.WorkspaceUserIds != null && obj.WorkspaceUserIds.Count > 0)
                        await UpdateWorkspaceUsers(obj.WorkspaceUserIds, existingObj.WorkspaceUsers.Select(x => x.UserId).ToList(), obj.Id);
                    
                    TempData["success"] = "Update workspace successfully";
                    return RedirectToAction("Update", "Workspace", new { id = obj.Id });
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

        private async Task<int> GetNewIdWorkspace()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var list = await GetWorkspaceByUserIdAsync(userId);
            if (list.Count > 0)
                return (int)(list.FirstOrDefault()?.Id);
            else return 0;
        }

        private async Task<List<UserDTO>> GetUsersAsync(string userId)
        {
            var response = await _userService.GetAllAsync<APIResponse>();
            List<UserDTO> users = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
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
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
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


        private async Task<List<WorkspaceDTO>> GetWorkspaceByUserIdAsync(string userId)
        {
            var response = await _workspaceService.GetByUserIdAsync<APIResponse>(userId);
            List<WorkspaceDTO> workspaces = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                workspaces = JsonConvert.DeserializeObject<List<WorkspaceDTO>>(Convert.ToString(response.Result));
            }
            if (workspaces.Count > 0)
            {
                workspaces = workspaces.OrderByDescending(x => x.CreatedDate)
                   .ThenByDescending(x => x.UpdatedDate)
                   .ToList();

                foreach (var item in workspaces)
                {
                    item.WorkspaceUsers = await GetWorkspaceUserByWorkspaceIdAsync(item.Id);
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

        private async Task<bool> UpdateWorkspaceUsers(List<string> userIdsNew, List<string> userIdsOld, int id)
        {
            try
            {

                var usersToAdd = userIdsNew.Except(userIdsOld).ToList();
                var usersToRemove = userIdsOld.Except(userIdsNew).ToList();

                foreach (var userId in usersToAdd)
                {
                    var response = await _workspaceUserService.CreateAsync<APIResponse>(new WorkspaceUserDTO { WorkspaceId = id, UserId = userId });
                    if (response == null && response.IsSuccess && response.ErrorMessages.Count == 0)
                    {
                        TempData["error"] = response.ErrorMessages.FirstOrDefault();
                        return false;
                    }

                }
                foreach (var userId in usersToRemove)
                {
                    var response = await _workspaceUserService.DeleteByWorkspaceAndUserAsync<APIResponse>(id, userId);
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

        private async Task<List<WorkspaceUserDTO>> GetWorkspaceUserByWorkspaceIdAsync(int id)
        {
            var response = await _workspaceUserService.GetByWorkspaceIdAsync<APIResponse>(id);
            List<WorkspaceUserDTO> obj = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                obj = JsonConvert.DeserializeObject<List<WorkspaceUserDTO>>(Convert.ToString(response.Result));
                if (obj != null && obj.Count > 0)
                {
                    foreach(var item in obj)
                    {
                        item.User = await GetUserByIdAsync(item.UserId);
                    }
                }
            }
            return obj;
        }
    }
}
