using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Security.Claims;
using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
using taskify_font_end.Models.VM;
using taskify_font_end.Service;
using taskify_font_end.Service.IService;

namespace taskify_font_end.Controllers
{
    public class TodoController : BaseController
    {


        private readonly IWorkspaceService _workspaceService;
        private readonly IUserService _userService;
        private readonly ITodoService _todoService;
        private readonly IColorService _colorService;
        private readonly IPriorityService _priorityService;

        public TodoController(ITodoService todoService,
            IWorkspaceService workspaceService, IUserService userService,
            IPriorityService priorityService, IColorService colorService) : base(workspaceService)
        {
            _todoService = todoService; 
            _colorService = colorService;
            _workspaceService = workspaceService;
            _userService = userService;
            _priorityService = priorityService;
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
            ViewBag.priorities = await GetPrioritiesByUserIdAsync(userId);

            List<TodoDTO> list = await GetTodoByUserIdAndWorkspaceIdAsync(userId, ViewBag.selectedWorkspaceId);
            return View(list);
        }



        [HttpPost]
        public async Task<IActionResult> CreateAsync(TodoDTO todoDTO)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(userId) || !todoDTO.UserId.Equals(userId))
                {
                    return RedirectToAction("AccessDenied", "Auth");
                }
                todoDTO.CreatedDate = DateTime.Now;
                APIResponse result = await _todoService.CreateAsync<APIResponse>(todoDTO);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                {
                    TempData["success"] = "Create new todo successfully";
                    return RedirectToAction("Index", "Todo");
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
            return RedirectToAction("Index", "Todo");
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
                APIResponse result = await _todoService.DeleteAsync<APIResponse>(id);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                    return Json(new { error = false, message = "Todo deleted successfully" });
                else
                    return Json(new { error = true, message = result?.ErrorMessages?.FirstOrDefault() ?? "An error occurred while deleting the todo" });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsync(TodoDTO todoDTO)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(userId) || !todoDTO.UserId.Equals(userId))
                {
                    return RedirectToAction("AccessDenied", "Auth");
                }
                todoDTO.UpdatedDate = DateTime.Now;
                APIResponse result = await _todoService.UpdateAsync<APIResponse>(todoDTO);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                {
                    TempData["success"] = "Update todo successfully";
                    return RedirectToAction("Index", "Todo");
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
            return RedirectToAction("Index", "Todo");
        }

        [HttpPut]
        public async Task<IActionResult> ChangeStatusAsync(StatusUpdateRequestVM request)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (ModelState.IsValid)
            {
                TodoDTO todo = await GetTodoById(request.Id);
                if (string.IsNullOrEmpty(userId) || !request.UserId.Equals(userId))
                {
                    return RedirectToAction("AccessDenied", "Auth");
                }
                todo.UpdatedDate = DateTime.Now;
                todo.Status = request.Status;
                APIResponse result = await _todoService.UpdateAsync<APIResponse>(todo);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                {
                    return Json(new { error = false, message = "Status updated successfully" });
                }
                else
                {
                    return Json(new { error = true, message = "Failed to update status" });
                }
            }
            else
            {
                var errorMessages = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage).FirstOrDefault();
                return Json(new { error = true, message = errorMessages });
            }
        }

        public async Task<IActionResult> Get(int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("AccessDenied", "Auth");
            TodoDTO obj = await GetTodoById(id);
            return Json(obj);
        }

        private async Task<List<TodoDTO>> GetTodoByUserIdAndWorkspaceIdAsync(string userId, int workspaceId)
        {
            var response = await _todoService.GetByUserIdAndWorkspaceIdAsync<APIResponse>(userId, workspaceId);
            List<TodoDTO> list = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                list = JsonConvert.DeserializeObject<List<TodoDTO>>(Convert.ToString(response.Result));
            }
            if (list.Count > 0)
            {
                list = list.OrderByDescending(x => x.UpdatedDate)
                   .ThenByDescending(x => x.CreatedDate)
                   .ToList();
                foreach (var item in list)
                {
                    item.Priority.Color = await GetColorById(item.Priority.ColorId);
                    
                }
            }
            return list;
        }

        private async Task<List<PriorityDTO>> GetPrioritiesByUserIdAsync(string userId)
        {
            var response = await _priorityService.GetByUserIdAsync<APIResponse>(userId);
            List<PriorityDTO> list = new();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<PriorityDTO>>(Convert.ToString(response.Result));
            }

            if (list.Count == 0)
                TempData["warning"] = "No priority are available.";

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

        private async Task<TodoDTO> GetTodoById(int id)
        {
            var response = await _todoService.GetAsync<APIResponse>(id);
            TodoDTO obj = new();
            if (response != null && response.IsSuccess)
            {
                obj = JsonConvert.DeserializeObject<TodoDTO>(Convert.ToString(response.Result));
            }

            if (obj == null)
                TempData["error"] = "Todo not found";

            return obj;
        }
    }
}
