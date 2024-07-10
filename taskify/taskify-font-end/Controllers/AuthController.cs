using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
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
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;
        private readonly IWorkspaceService _workspaceService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IWorkspaceUserService _workspaceUserService;
        public AuthController(IAuthService authService, ITokenProvider tokenProvider,
            IWorkspaceService workspaceService, IUserService userService, IWorkspaceUserService workspaceUserService,
            IMapper mapper) : base(workspaceService, workspaceUserService)
        {
            _workspaceUserService = workspaceUserService;
            _authService = authService;
            _tokenProvider = tokenProvider;
            _workspaceService = workspaceService;
            _userService = userService;
            _mapper = mapper;
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

        public IActionResult LoginGoogle()
        {
            var authenticationProperties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(authenticationProperties, GoogleDefaults.AuthenticationScheme);
        }
        public async Task<IActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
                return BadRequest();

            var token = authenticateResult.Properties.GetTokenValue("access_token");
            if (!string.IsNullOrEmpty(token))
            {
                var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
                var firstName = authenticateResult.Principal.FindFirst(ClaimTypes.GivenName)?.Value;
                var lastName = authenticateResult.Principal.FindFirst(ClaimTypes.Surname)?.Value;
                var address = authenticateResult.Principal.FindFirst(ClaimTypes.StreetAddress)?.Value;

                var googleAuthDto = new GoogleAuthDTO
                {
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    Address = address,
                    Password = email
                };
                var response = await _authService.AuthenticateWithGoogle<APIResponse>(googleAuthDto);
                if (response != null && response.IsSuccess)
                {
                    TokenDTO model = JsonConvert.DeserializeObject<TokenDTO>(Convert.ToString(response.Result));

                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(model.AccessToken);

                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == "unique_name").Value));
                    identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, jwt.Claims.FirstOrDefault(u => u.Type == "sub").Value));
                    identity.AddClaim(new Claim(ClaimTypes.GivenName, jwt.Claims.FirstOrDefault(u => u.Type == "given_name").Value));
                    identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, "google"));
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                    {
                        IsPersistent = true, // Persistent cookie
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Cookie expiration set to 30 minutes
                    });
                    _tokenProvider.SetToken(model);
                    List<WorkspaceDTO> workspaces = await GetAllWorkspaceByUserIdAsync(jwt.Claims.FirstOrDefault(u => u.Type == "sub").Value);
                    if (workspaces.Count > 0)
                    {
                        HttpContext.Response.Cookies.Append("SelectedWorkspaceId", workspaces.FirstOrDefault().Id.ToString());
                        return RedirectToAction("Dashboard", "Home", new { id = workspaces.FirstOrDefault().Id });
                    }
                    else
                    {
                        HttpContext.Response.Cookies.Append("SelectedWorkspaceId", "0");
                        return RedirectToAction("Dashboard", "Home", new { id = 0 });
                    }
                    

                }
                else
                {
                    TempData["error"] = response.ErrorMessages.FirstOrDefault();
                    return RedirectToAction("Login", "Auth");
                }
            }
            else
            {
                TempData["error"] = "Token is null";
                return RedirectToAction("Login", "Auth");
            }
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> ProfileAsync(string id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !id.Equals(userId))
            {
                return RedirectToAction("AccessDenied", "Auth");
            }
            UserDTO user = await GetUserByIdAsync(id);
            var obj = _mapper.Map<UserUpdateDTO>(user);
            return View(obj);
        }

        [HttpPost]
        public async Task<IActionResult> UploadImg(UserUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO.Image == null)
                {
                    TempData["error"] = "Please upload image avatar";
                    return RedirectToAction("Profile", "Auth", new { id = updateDTO.Id });
                }
                var obj = await GetUserByIdAsync(updateDTO.Id);
                CopyAttributes(obj, updateDTO);
                var user = _mapper.Map<UserDTO>(updateDTO);
                var response = await _userService.UploadImgAsync<APIResponse>(user);
                if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
                {
                    TempData["success"] = "Update Image successful!";
                }
                else
                {
                    TempData["error"] = response.ErrorMessages[0];
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }
            return RedirectToAction("Profile", "Auth", new { id = updateDTO.Id });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfileAsync(UserUpdateDTO updateDTO)
        {
            try
            {
                string msg = string.Empty;
                if (string.IsNullOrEmpty(updateDTO.NewPassword) && string.IsNullOrEmpty(updateDTO.ConfirmNewPassword) && string.IsNullOrEmpty(updateDTO.OldPassword))
                {
                    var obj = await GetUserByIdAsync(updateDTO.Id);
                }
                else if (updateDTO.NewPassword != updateDTO.ConfirmNewPassword)
                {
                    TempData["error"] = "Passwords do not match.";
                    return RedirectToAction("Profile", "Auth", new { id = updateDTO.Id });
                }
                else
                {

                    var user = await GetUserByIdAsync(updateDTO.Id);
                    UpdatePasswordRequestDTO updatePasswordRequestDTO = new UpdatePasswordRequestDTO() { Id = updateDTO.Id, NewPassword = updateDTO.NewPassword, Password = updateDTO.OldPassword, UserName = updateDTO.Email };
                    var changePasswordResult = await _userService.UpdatePasswordAsync<APIResponse>(updatePasswordRequestDTO);
                    if (changePasswordResult != null && changePasswordResult.IsSuccess && changePasswordResult.ErrorMessages.Count == 0)
                    {
                        msg += "Update password successfull\n";
                    }
                    else
                    {
                        TempData["error"] = "Current password not correct.";
                        return RedirectToAction("Profile", "Auth", new { id = updateDTO.Id });
                    }
                }

                var userDTO = _mapper.Map<UserDTO>(updateDTO);
                var response = await _userService.UpdateAsync<APIResponse>(userDTO);
                if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
                {
                    msg += "Update profile successful!";
                    TempData["success"] = msg;
                }
                else
                {
                    TempData["error"] = response.ErrorMessages[0];
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }

            return RedirectToAction("Profile", "Auth", new { id = updateDTO.Id });
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
                    identity.AddClaim(new Claim(ClaimTypes.GivenName, jwt.Claims.FirstOrDefault(u => u.Type == "given_name").Value));
                    identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, "user"));
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                    {
                        IsPersistent = true, // Persistent cookie
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Cookie expiration set to 30 minutes
                    });
                    _tokenProvider.SetToken(model);
                    if (ViewBag.SelectedWorkspaceId == null || ViewBag.SelectedWorkspaceId == 0)
                    {
                        List<WorkspaceDTO> workspaces = await GetAllWorkspaceByUserIdAsync(jwt.Claims.FirstOrDefault(u => u.Type == "sub").Value);
                        if (workspaces.Count > 0)
                        {
                            HttpContext.Response.Cookies.Append("SelectedWorkspaceId", workspaces.FirstOrDefault().Id.ToString());
                            return RedirectToAction("Dashboard", "Home", new { id = workspaces.FirstOrDefault().Id });
                        }
                        else
                        {
                            HttpContext.Response.Cookies.Append("SelectedWorkspaceId", "0");
                            return RedirectToAction("Dashboard", "Home", new { id = 0 });
                        }
                    }
                    else
                    {
                        return RedirectToAction("Dashboard", "Home", new { id = ViewBag.SelectedWorkspaceId });
                    }

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
                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
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
            var res = await _workspaceService.GetByUserIdAsync<APIResponse>(userId);
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


        private async Task<UserDTO> GetUserByIdAsync(string userId)
        {
            var response = await _userService.GetAsync<APIResponse>(userId);
            UserDTO user = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                user = JsonConvert.DeserializeObject<UserDTO>(Convert.ToString(response.Result));
            }
            return user;
        }

        private void CopyAttributes(UserDTO source, UserUpdateDTO destination)
        {
            destination.Id = source.Id;
            destination.Email = source.Email;
            destination.FirstName = source.FirstName;
            destination.PhoneNumber = source.PhoneNumber;
            destination.LastName = source.LastName;
            destination.Address = source.Address;
            destination.ImageUrl = source.ImageUrl;
            destination.ImageLocalPathUrl = source.ImageLocalPathUrl;
        }

    }
}
