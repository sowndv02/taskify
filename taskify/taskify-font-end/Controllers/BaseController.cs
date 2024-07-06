using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Security.Claims;
using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
using taskify_font_end.Service;
using taskify_font_end.Service.IService;

namespace taskify_font_end.Controllers
{

    public class BaseController : Controller
    {
        private readonly IWorkspaceService _workspaceService;
        private readonly IWorkspaceUserService _workspaceUserService;
        private string USER_ID;
        public BaseController(IWorkspaceService workspaceService, IWorkspaceUserService workspaceUserService)
        {
            _workspaceService = workspaceService;
            _workspaceUserService = workspaceUserService;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            USER_ID = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(USER_ID))
            {
                var workspaces = await GetWorkspaceByUserIdAsync(USER_ID);
                ViewBag.workspaces = workspaces;
                ViewBag.userId = USER_ID;

                if (HttpContext.Request.Cookies.TryGetValue("SelectedWorkspaceId", out var selectedWorkspaceId))
                {
                    if (int.TryParse(selectedWorkspaceId, out var workspaceId))
                    {
                        ViewBag.SelectedWorkspaceId = workspaceId;
                    }
                }
            }
            await next();
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

    }
}
