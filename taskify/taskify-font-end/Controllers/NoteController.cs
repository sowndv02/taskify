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
    public class NoteController : BaseController
    {
        private readonly IWorkspaceService _workspaceService;
        private readonly IUserService _userService;
        private readonly IColorService _colorService;
        private readonly INoteService _noteService;
        private readonly IMapper _mapper;

        public NoteController(
            IColorService colorService, INoteService noteService, 
            IUserService userService, IWorkspaceService workspaceService, 
            IMapper mapper) : base(workspaceService)
        {
            _colorService = colorService;
            _noteService = noteService;
            _userService = userService;
            _workspaceService = workspaceService;
            _mapper = mapper;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("AccessDenied", "Auth");
            if (ViewBag.selectedWorkspaceId == null || ViewBag.selectedWorkspaceId == 0)
            {
                TempData["error"] = "You don't have any workspaces! Please create a workspace first!";
                return RedirectToAction("Create", "Workspace");
            }
            List<NoteDTO> list = await GetNoteByWorkspaceIdAsync(ViewBag.selectedWorkspaceId);

            ViewBag.colors = await GetColorssByUserIdAsync(userId);

            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(NoteDTO noteDTO)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(userId) || !noteDTO.UserId.Equals(userId))
                {
                    return RedirectToAction("AccessDenied", "Auth");
                }
                noteDTO.CreatedDate = DateTime.Now;
                APIResponse result = await _noteService.CreateAsync<APIResponse>(noteDTO);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                {
                    TempData["success"] = "Create new note successfully";
                    return RedirectToAction("Index", "Note");
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
            return RedirectToAction("Index", "Note");
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
                
                APIResponse result = await _noteService.DeleteAsync<APIResponse>(id);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                    return Json(new { error = false, message = "Note deleted successfully" });
                else
                    return Json(new { error = true, message = result?.ErrorMessages?.FirstOrDefault() ?? "An error occurred while deleting the note" });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsync(NoteDTO noteDTO)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(userId) || !noteDTO.UserId.Equals(userId))
                {
                    return RedirectToAction("AccessDenied", "Auth");
                }
                noteDTO.UpdatedDate = DateTime.Now;
                APIResponse result = await _noteService.UpdateAsync<APIResponse>(noteDTO);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                {
                    TempData["success"] = "Update note successfully";
                    return RedirectToAction("Index", "Note");
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
            return RedirectToAction("Index", "Note");
        }

        public async Task<IActionResult> Get(int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("AccessDenied", "Auth");
            NoteDTO obj = await GetNoteById(id);
            if(obj == null) return Json(new { error = true, message = "Note not found!" });
            return Json(obj);
        }


        private async Task<List<NoteDTO>> GetNoteByWorkspaceIdAsync(int id)
        {
            var response = await _noteService.GetByWorkspaceIdAsync<APIResponse>(id);
            List<NoteDTO> list = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                list = JsonConvert.DeserializeObject<List<NoteDTO>>(Convert.ToString(response.Result));
            }
            if (list.Count > 0)
            {
                list = list.OrderByDescending(x => x.CreatedDate)
                   .ThenByDescending(x => x.UpdatedDate)
                   .ToList();
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
        private async Task<NoteDTO> GetNoteById(int id)
        {
            var response = await _noteService.GetAsync<APIResponse>(id);
            NoteDTO obj = new();
            if (response != null && response.IsSuccess)
            {
                obj = JsonConvert.DeserializeObject<NoteDTO>(Convert.ToString(response.Result));
            }
            return obj;
        }
    }
}
