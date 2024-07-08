using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
using taskify_font_end.Service.IService;

namespace taskify_font_end.Controllers
{
    public class ActivityController : BaseController
    {
        private readonly IWorkspaceService _workspaceService;
        private readonly IWorkspaceUserService _workspaceUserService;
        private readonly IActivityService _activityService;
        private readonly IMapper _mapper;

        public ActivityController(IMapper mapper,
            IWorkspaceService workspaceService, 
            IWorkspaceUserService workspaceUserService,
            IActivityService activityService) : base(workspaceService, workspaceUserService)
        {
            _workspaceUserService = workspaceUserService;
            _workspaceService = workspaceService;
            _mapper = mapper;
            _activityService = activityService;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("AccessDenied", "Auth");
            try
            {
                List<ActivityDTO> list = await GetActivitiesAsync();
                if (list.Count == 0) TempData["warning"] = "Activity is empty";
                return View(list);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("Dashboard", "Home");
            }
        }

        public async Task<IActionResult> ListAsync()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("AccessDenied", "Auth");
            List<ActivityDTO> list = await GetActivitiesAsync();
            return Json(list);
        }

        public async Task<IActionResult> Get(int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("AccessDenied", "Auth");
            ActivityDTO obj = await GetActivityById(id);
            return Json(obj);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(ActivityDTO activityDTO)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("AccessDenied", "Auth");
                }

                APIResponse result = await _activityService.CreateAsync<APIResponse>(activityDTO);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                {
                    TempData["success"] = "Create new activity successfully";
                    return RedirectToAction("Index", "Activity");
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
            return RedirectToAction("Index", "Activity");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsync(ActivityDTO activityDTO)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("AccessDenied", "Auth");
                }

                APIResponse result = await _activityService.UpdateAsync<APIResponse>(activityDTO);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                {
                    TempData["success"] = "Update activity successfully";
                    return RedirectToAction("Index", "Activity");
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
            return RedirectToAction("Index", "Activity");
        }

        private async Task<List<ActivityDTO>> GetActivitiesAsync()
        {
            var response = await _activityService.GetAllAsync<APIResponse>();
            List<ActivityDTO> list = new();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ActivityDTO>>(Convert.ToString(response.Result));
            }
            return list;
        }

        private async Task<ActivityDTO> GetActivityById(int id)
        {
            var response = await _activityService.GetAsync<APIResponse>(id);
            ActivityDTO obj = new();
            if (response != null && response.IsSuccess)
            {
                obj = JsonConvert.DeserializeObject<ActivityDTO>(Convert.ToString(response.Result));
            }
            return obj;
        }

    }
}
