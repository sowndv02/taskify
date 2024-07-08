using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
using taskify_font_end.Service.IService;

namespace taskify_font_end.Controllers
{
    public class ColorController : BaseController
    {
        private readonly IProjectService _projectService;
        private readonly IWorkspaceService _workspaceService;
        private readonly IWorkspaceUserService _workspaceUserService;
        private readonly IUserService _userService;
        private readonly IStatusService _statusService;
        private readonly ITagService _tagService;
        private readonly IProjectUserService _projectUserService;
        private readonly IProjectTagService _projectTagService;
        private readonly IColorService _colorService;
        private readonly INoteService _noteService;
        private readonly IPriorityService _priorityService;
        private readonly IMapper _mapper;

        public ColorController(IProjectService projectService, IMapper mapper,
            IWorkspaceService workspaceService, IUserService userService,
            IStatusService statusService, ITagService tagService,
            IProjectUserService projectUserService, IProjectTagService projectTagService,
            IConfiguration configuration, IColorService colorService, 
            IWorkspaceUserService workspaceUserService, INoteService noteService, 
            IPriorityService priorityService) : base(workspaceService, workspaceUserService)
        {
            _workspaceUserService = workspaceUserService;
            _workspaceService = workspaceService;
            _mapper = mapper;
            _projectService = projectService;
            _userService = userService;
            _statusService = statusService;
            _tagService = tagService;
            _projectUserService = projectUserService;
            _projectTagService = projectTagService;
            _colorService = colorService;
            _noteService = noteService;
            _priorityService = priorityService;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("AccessDenied", "Auth");
            try
            {
                List<ColorDTO> list = await GetColorssByUserIdAsync(userId);
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
            List<ColorDTO> list = await GetColorssByUserIdAsync(userId);
            return Json(list);
        }

        public async Task<IActionResult> Get(int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("AccessDenied", "Auth");
            ColorDTO obj = await GetColorById(id);
            return Json(obj);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(ColorDTO colorDTO)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(userId) || !colorDTO.UserId.Equals(userId))
                {
                    return RedirectToAction("AccessDenied", "Auth");
                }

                APIResponse result = await _colorService.CreateAsync<APIResponse>(colorDTO);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                {
                    TempData["success"] = "Create new color successfully";
                    return RedirectToAction("Index", "Color");
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
            return RedirectToAction("Index", "Color");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                return Json(new { error = true, message = "Invalid ID" });
            }
            try
            {
                // Note
                var notes = await GetNoteByColorId(id);
                if (notes != null && notes.Count > 0)
                {
                    return Json(new { error = true, message = "Color is being used by some note. Please delete the note before removing the color!" });
                }
                //Tag
                var tags = await GetTagByColorId(id);
                if (tags != null && tags.Count > 0)
                {
                    return Json(new { error = true, message = "Color is being used by some tag. Please delete the tag before removing the color!" });
                }
                //Status
                var statuses = await GetStatusByColorId(id);
                if (statuses != null && statuses.Count > 0)
                {
                    return Json(new { error = true, message = "Color is being used by some status. Please delete the status before removing the color!" });
                }
                //Priority
                var priorities = await GetPriorityByColorId(id);
                if (priorities != null && priorities.Count > 0)
                {
                    return Json(new { error = true, message = "Color is being used by some priority. Please delete the priority before removing the color!" });
                }

                APIResponse result = await _colorService.DeleteAsync<APIResponse>(id);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                    return Json(new { error = false, message = "Color deleted successfully" });
                else
                    return Json(new { error = true, message = result?.ErrorMessages?.FirstOrDefault() ?? "An error occurred while deleting the tag" });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsync(ColorDTO colorDTO)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(userId) || !colorDTO.UserId.Equals(userId))
                {
                    return RedirectToAction("AccessDenied", "Auth");
                }

                APIResponse result = await _colorService.UpdateAsync<APIResponse>(colorDTO);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                {
                    TempData["success"] = "Update color successfully";
                    return RedirectToAction("Index", "Color");
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
            return RedirectToAction("Index", "Color");
        }


        private async Task<List<NoteDTO>> GetNoteByColorId(int id)
        {
            var response = await _noteService.GetByColorIdAsync<APIResponse>(id);
            List<NoteDTO> list = new();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<NoteDTO>>(Convert.ToString(response.Result));
            }
            return list;
        }

        private async Task<List<ColorDTO>> GetColorssByUserIdAsync(string userId)
        {
            var response = await _colorService.GetByUserIdAsync<APIResponse>(userId);
            List<ColorDTO> list = new();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ColorDTO>>(Convert.ToString(response.Result));
            }

            if (list.Count == 0)
                TempData["warning"] = "No color are available.";

            return list;
        }

        private async Task<ColorDTO> GetColorById(int id)
        {
            var response = await _colorService.GetAsync<APIResponse>(id);
            ColorDTO obj = new();
            if (response != null && response.IsSuccess)
            {
                obj = JsonConvert.DeserializeObject<ColorDTO>(Convert.ToString(response.Result));
            }

            if (obj == null)
                TempData["error"] = "Color not found";

            return obj;
        }


        private async Task<List<PriorityDTO>> GetPriorityByColorId(int id)
        {
            var response = await _priorityService.GetByColorIdAsync<APIResponse>(id);
            List<PriorityDTO> list = new();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<PriorityDTO>>(Convert.ToString(response.Result));
            }
            return list;
        }

        private async Task<List<StatusDTO>> GetStatusByColorId(int id)
        {
            var response = await _statusService.GetByColorIdAsync<APIResponse>(id);
            List<StatusDTO> list = new();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<StatusDTO>>(Convert.ToString(response.Result));
            }
            return list;
        }

        private async Task<List<TagDTO>> GetTagByColorId(int id)
        {
            var response = await _tagService.GetByColorIdAsync<APIResponse>(id);
            List<TagDTO> list = new();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<TagDTO>>(Convert.ToString(response.Result));
            }
            return list;
        }
    }
}
