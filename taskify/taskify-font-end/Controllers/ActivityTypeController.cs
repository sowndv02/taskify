using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
using taskify_font_end.Service.IService;

namespace taskify_font_end.Controllers
{
    public class ActivityTypeController : BaseController
    {
        private readonly IWorkspaceService _workspaceService;
        private readonly IWorkspaceUserService _workspaceUserService;
        private readonly IActivityTypeService _activityTypeService;
        private readonly IMapper _mapper;

        public ActivityTypeController(IMapper mapper,
            IWorkspaceService workspaceService,
            IWorkspaceUserService workspaceUserService,
            IActivityTypeService activityTypeService) : base(workspaceService, workspaceUserService)
        {
            _workspaceUserService = workspaceUserService;
            _workspaceService = workspaceService;
            _mapper = mapper;
            _activityTypeService = activityTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("AccessDenied", "Auth");
            try
            {
                List<ActivityTypeDTO> list = await GetActivityTypesAsync();
                if (list.Count == 0) TempData["warning"] = "Activity type is empty";
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
            List<ActivityTypeDTO> list = await GetActivityTypesAsync();
            return Json(list);
        }

        public async Task<IActionResult> Get(int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("AccessDenied", "Auth");
            ActivityTypeDTO obj = await GetActivityTypeById(id);
            return Json(obj);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(ActivityTypeDTO activityTypeDTO)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("AccessDenied", "Auth");
                }

                APIResponse result = await _activityTypeService.CreateAsync<APIResponse>(activityTypeDTO);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                {
                    TempData["success"] = "Create new activity successfully";
                    return RedirectToAction("Index", "ActivityType");
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
            return RedirectToAction("Index", "ActivityType");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsync(ActivityTypeDTO activityTypeDTO)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("AccessDenied", "Auth");
                }

                APIResponse result = await _activityTypeService.UpdateAsync<APIResponse>(activityTypeDTO);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                {
                    TempData["success"] = "Update activity successfully";
                    return RedirectToAction("Index", "ActivityType");
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

        private async Task<List<ActivityTypeDTO>> GetActivityTypesAsync()
        {
            var response = await _activityTypeService.GetAllAsync<APIResponse>();
            List<ActivityTypeDTO> list = new();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ActivityTypeDTO>>(Convert.ToString(response.Result));
            }
            return list;
        }

        private async Task<ActivityTypeDTO> GetActivityTypeById(int id)
        {
            var response = await _activityTypeService.GetAsync<APIResponse>(id);
            ActivityTypeDTO obj = new();
            if (response != null && response.IsSuccess)
            {
                obj = JsonConvert.DeserializeObject<ActivityTypeDTO>(Convert.ToString(response.Result));
            }
            return obj;
        }
    }
}
