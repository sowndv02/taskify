using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
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
        private readonly IMapper _mapper;
        public HomeController(IWorkspaceService workspaceService, IMapper mapper, IProjectService projectService) : base(workspaceService)
        {
            _workspaceService = workspaceService;
            _mapper = mapper;
            _projectService = projectService;
        }

        public IActionResult LandingPage()
        {
            return View();
        }


        public async Task<IActionResult> DashboardAsync(int? id)
        {
            if(ViewBag.SelectedWorkspaceId == 0 && id != 0)
                HttpContext.Response.Cookies.Append("SelectedWorkspaceId", id.ToString());
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
                if (id == null || id == 0)
                {
                    if (workspaces.Count > 0)
                    {
                        var projects = await GetProjectByUserIdAndWorkspaceIdAsync(userId, workspaces.First().Id);
                        ViewBag.totalProjects = projects.Count;
                        return View(workspaces.First());
                    }
                    return View(null);
                }
                var workspace = workspaces.FirstOrDefault(x => x.Id == id);
                if(workspace != null)
                {
                    var projects = await GetProjectByUserIdAndWorkspaceIdAsync(userId, workspace.Id);
                    ViewBag.totalProjects = projects.Count;
                }
                return View(workspace);
            }
            return View(null);
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
            }
            if (workspaces.Count > 0)
            {
                workspaces = workspaces.OrderByDescending(x => x.CreatedDate)
                   .ThenByDescending(x => x.UpdatedDate)
                   .ToList();
            }
            return workspaces;
        }



        private async Task<List<ProjectDTO>> GetProjectByUserIdAndWorkspaceIdAsync(string userId, int workspaceId)
        {
            var response = await _projectService.GetByUserIdAndWorkspaceIdAsync<APIResponse>(userId, workspaceId);
            List<ProjectDTO> list = new();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProjectDTO>>(Convert.ToString(response.Result));
            }
            return list;
        }

    }
}