using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
using taskify_font_end.Service.IService;

namespace taskify_font_end.Controllers
{
    public class TagController : BaseController
    {
        private readonly IProjectService _projectService;
        private readonly IWorkspaceService _workspaceService;
        private readonly IUserService _userService;
        private readonly IStatusService _statusService;
        private readonly ITagService _tagService;
        private readonly IProjectUserService _projectUserService;
        private readonly IProjectTagService _projectTagService;
        private readonly IColorService _colorService;
        private readonly IMapper _mapper;

        public TagController(IProjectService projectService, IMapper mapper,
            IWorkspaceService workspaceService, IUserService userService,
            IStatusService statusService, ITagService tagService,
            IProjectUserService projectUserService, IProjectTagService projectTagService, 
            IConfiguration configuration, IColorService colorService) : base(workspaceService)
        {
            _workspaceService = workspaceService;
            _mapper = mapper;
            _projectService = projectService;
            _userService = userService;
            _statusService = statusService;
            _tagService = tagService;
            _projectUserService = projectUserService;
            _projectTagService = projectTagService;
            _colorService = colorService;

        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("AccessDenied", "Auth");
            try
            {
                List<TagDTO> list = await GetTagsByUserIdAsync(userId);
                ViewBag.colors = await GetColorssByUserIdAsync(userId);
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
            List<TagDTO> list = await GetTagsByUserIdAsync(userId);
            return Json(list);
        }

        public async Task<IActionResult> Get(int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("AccessDenied", "Auth");
            TagDTO obj = await GetTagById(id);
            return Json(obj);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(TagDTO tagDTO) {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(userId) || !tagDTO.UserId.Equals(userId))
                {
                    return RedirectToAction("AccessDenied", "Auth");
                }

                APIResponse result = await _tagService.CreateAsync<APIResponse>(tagDTO);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                {
                    TempData["success"] = "Create new tag successfully";
                    return RedirectToAction("Index", "Tag");
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
            return RedirectToAction("Index", "Tag");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if(id <= 0)
            {
                return Json(new { error = true, message = "Invalid ID" });
            }
            try
            {
                var projectTags = await GetProjectTagByTagId(id);
                if(projectTags!= null && projectTags.Count > 0)
                {
                    return Json(new { error = true, message = "Tag is being used by some project. Please delete the project before removing the tag!" });
                }
                APIResponse result = await _tagService.DeleteAsync<APIResponse>(id);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                    return Json(new { error = false, message = "Tag deleted successfully" });
                else
                    return Json(new { error = true, message = result?.ErrorMessages?.FirstOrDefault() ?? "An error occurred while deleting the tag" });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsync(TagDTO tagDTO)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(userId) || !tagDTO.UserId.Equals(userId))
                {
                    return RedirectToAction("AccessDenied", "Auth");
                }

                APIResponse result = await _tagService.UpdateAsync<APIResponse>(tagDTO);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                {
                    TempData["success"] = "Update tag successfully";
                    return RedirectToAction("Index", "Tag");
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
            return RedirectToAction("Index", "Tag");
        }


        private async Task<List<TagDTO>> GetTagsByUserIdAsync(string userId)
        {
            var response = await _tagService.GetByUserIdAsync<APIResponse>(userId);
            List<TagDTO> list = new();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<TagDTO>>(Convert.ToString(response.Result));
            }

            if (list.Count == 0)
                TempData["warning"] = "No tag are available.";

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

        private async Task<TagDTO> GetTagById(int id)
        {
            var response = await _tagService.GetAsync<APIResponse>(id);
           TagDTO obj = new();
            if (response != null && response.IsSuccess)
            {
                obj = JsonConvert.DeserializeObject<TagDTO>(Convert.ToString(response.Result));
            }

            if (obj == null)
                TempData["error"] = "Tag not found";

            return obj;
        }

        private async Task<List<ProjectTagDTO>> GetProjectTagByTagId(int id)
        {
            var response = await _projectTagService.GetByTagIdAsync<APIResponse>(id);
            List<ProjectTagDTO> obj = new();
            if (response != null && response.IsSuccess)
            {
                obj = JsonConvert.DeserializeObject<List<ProjectTagDTO>>(Convert.ToString(response.Result));
            }
            return obj;
        }

    }
}
