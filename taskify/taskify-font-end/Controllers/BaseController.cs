using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Security.Claims;
using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
using taskify_font_end.Service.IService;

namespace taskify_font_end.Controllers
{

    public class BaseController : Controller
    {
        private readonly IWorkspaceService _workspaceService;
        private string USER_ID;
        public BaseController(IWorkspaceService workspaceService)
        {
            _workspaceService = workspaceService;
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
