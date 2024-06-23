using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
using taskify_font_end.Service.IService;

namespace taskify_font_end.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWorkspaceService _workspaceService;
        private readonly IMapper _mapper;
        public HomeController(IWorkspaceService workspaceService, IMapper mapper)
        {
            _workspaceService = workspaceService;
            _mapper = mapper;
        }

        public IActionResult LandingPage()
        {
            return View();
        }

        public async Task<IActionResult> DashboardAsync(int id)
        {
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
                if (id == 0)
                {
                    if (workspaces.Count > 0)
                    {
                        return View(workspaces.First());
                    }
                    return View(null);
                }
                var workspace = workspaces.FirstOrDefault(x => x.Id == id);
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

        //private async Task<List<ProjectDTO> GetProjectByUserIdAsync(string userId)
        //{
        //    var response = await _projectService.GetByUserIdAsync<APIResponse>(userId);
        //    List<ProjectDTO> projects = new();
        //    if (response != null && response.IsSuccess)
        //    {
        //        projects = JsonConvert.DeserializeObject<List<ProjectDTO>>(Convert.ToString(response.Result));
        //    }
        //    return projects;
        //}

    }
}