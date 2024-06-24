﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
using taskify_font_end.Models.VM;
using taskify_font_end.Service.IService;
using taskify_utility;

namespace taskify_font_end.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;
        private readonly IWorkspaceService _workspaceService;
        public AuthController(IAuthService authService, ITokenProvider tokenProvider, IWorkspaceService workspaceService)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
            _workspaceService = workspaceService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            AuthVM obj = new()
            {
                LoginRequest = new LoginRequestDTO(),
                RegisterationRequest = new RegisterationRequestDTO()
            };
            return View(obj);
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }


        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AuthVM obj)
        {
            if (ModelState.IsValid)
            {
                APIResponse response = await _authService.LoginAsync<APIResponse>(obj.LoginRequest);
                if (response != null && response.IsSuccess)
                {
                    TokenDTO model = JsonConvert.DeserializeObject<TokenDTO>(Convert.ToString(response.Result));

                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(model.AccessToken);

                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == "unique_name").Value));
                    identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, jwt.Claims.FirstOrDefault(u => u.Type == "sub").Value));
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    _tokenProvider.SetToken(model);
                    List<WorkspaceDTO> workspaces = await GetAllWorkspaceByUserIdAsync(jwt.Claims.FirstOrDefault(u => u.Type == "sub").Value);
                    if (workspaces.Count > 0)
                        return RedirectToAction("Dashboard", "Home", workspaces.FirstOrDefault().Id);
                    else
                        return RedirectToAction("Dashboard", "Home", 0);
                }
                else
                {
                    TempData["error"] = response.ErrorMessages.FirstOrDefault();
                    return View(obj);
                }
            }
            else
            {
                var errorMessages = ModelState.Values.SelectMany(v => v.Errors)
                                                 .Select(e => e.ErrorMessage).FirstOrDefault();
                TempData["error"] = errorMessages;
            }
            return View(obj);
        }


        [HttpGet]
        public IActionResult Register()
        {
            return RedirectToAction("Login", "Auth");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AuthVM obj)
        {

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(obj.RegisterationRequest.Role))
                {
                    obj.RegisterationRequest.Role = SD.Client;
                }
                APIResponse result = await _authService.RegisterAsync<APIResponse>(obj.RegisterationRequest);
                if (result != null && result.IsSuccess)
                {
                    TempData["success"] = "Register successfully";
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
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            var token = _tokenProvider.GetToken();
            await _authService.LogoutAsync<APIResponse>(token);
            _tokenProvider.ClearToken();
            return RedirectToAction("LandingPage", "Home");
        }
        private async Task<List<WorkspaceDTO>> GetAllWorkspaceByUserIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return new List<WorkspaceDTO>();
            var res = await _workspaceService.GetAllAsync<APIResponse>();
            List<WorkspaceDTO> workspaces = new List<WorkspaceDTO>();
            if (res != null && res.IsSuccess)
            {
                workspaces = JsonConvert.DeserializeObject<List<WorkspaceDTO>>(Convert.ToString(res.Result));
            }
            if (workspaces.Count > 0)
            {
                workspaces.OrderByDescending(x => x.CreatedDate);
            }
            return workspaces;
        }
    }
}
