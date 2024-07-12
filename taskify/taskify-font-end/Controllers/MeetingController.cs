using AutoMapper;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
using taskify_font_end.Service.IService;
using taskify_utility;

namespace taskify_font_end.Controllers
{
    public class MeetingController : BaseController
    {
        private readonly IMeetingService _meetingService;
        private readonly IWorkspaceService _workspaceService;
        private readonly IWorkspaceUserService _workspaceUserService;
        private readonly IUserService _userService;
        private readonly IMeetingUserService _meetingUserService;
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;
        private readonly IMapper _mapper;

        public MeetingController(IMeetingService meetingService, IMapper mapper,
            IWorkspaceService workspaceService, IUserService userService,
            IStatusService statusService, ITagService tagService,
            IMeetingUserService meetingUserService,
            IWorkspaceUserService workspaceUserService,
            IAuthService authService,
            ITokenProvider tokenProvider,
            IConfiguration configuration) : base(workspaceService, workspaceUserService)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
            _workspaceUserService = workspaceUserService;
            _workspaceService = workspaceService;
            _mapper = mapper;
            _meetingService = meetingService;
            _userService = userService;
            _meetingUserService = meetingUserService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("AccessDenied", "Auth");
            if (ViewBag.selectedWorkspaceId == null || ViewBag.selectedWorkspaceId == 0)
            {
                TempData["error"] = "You don't have any workspaces! Please create a workspace first!";
                return RedirectToAction("Create", "Workspace");
            }
            List<MeetingDTO> list = await GetMeetingByWorkspaceIdAsync(ViewBag.selectedWorkspaceId);
            return View(list);
        }

        public async Task<IActionResult> Update(int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("AccessDenied", "Auth");
            }
            var accessToken = User.Claims.FirstOrDefault(c => c.Type == "access_token_google")?.Value;
            if (string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction("LoginGoogle", "Meeting");
            }

            MeetingDTO meeting = await GetMeetingByIdAsync(id);

            //StartDate = StartDateTime.Date;
            //StartTime = DateTime.Today.Add(StartDateTime.TimeOfDay);
            //EndDate = EndDateTime.Date;
            //EndTime = DateTime.Today.Add(EndDateTime.TimeOfDay);

            ViewBag.users = await GetUsersByWorkspaceIdAsync(userId, ViewBag.selectedWorkspaceId);
            return View();
        }

        public async Task<IActionResult> Create()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("AccessDenied", "Auth");
            }
            var accessToken = User.Claims.FirstOrDefault(c => c.Type == "access_token_google")?.Value;
            if (string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction("LoginGoogle", "Meeting");
            }

            ViewBag.users = await GetUsersByWorkspaceIdAsync(userId, ViewBag.selectedWorkspaceId);
            MeetingDTO meeting = new MeetingDTO();
            return View(meeting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(MeetingDTO obj)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("AccessDenied", "Auth");
            }
            List<UserDTO> users = await GetUsersByWorkspaceIdAsync(userId, ViewBag.selectedWorkspaceId);
            ViewBag.users = users;
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(userId) || !obj.OwnerId.Equals(userId))
                {
                    return RedirectToAction("AccessDenied", "Auth");
                }
                if (ViewBag.selectedWorkspaceId == null || ViewBag.selectedWorkspaceId == 0)
                {
                    TempData["error"] = "You don't have any workspaces! Please create a workspace first!";
                    return RedirectToAction("Create", "Workspace");
                }
                obj.WorkspaceId = ViewBag.selectedWorkspaceId;
                var accessToken = User.Claims.FirstOrDefault(c => c.Type == "access_token_google")?.Value;
                if (string.IsNullOrEmpty(accessToken))
                {
                    return RedirectToAction("LoginGoogle", "Meeting");
                }
                obj.StartDateTime = obj.StartDate.Date.Add(obj.StartTime.TimeOfDay);
                obj.EndDateTime = obj.EndDate.Date.Add(obj.EndTime.TimeOfDay);
                obj.Token = accessToken;
                var credential = GoogleCredential.FromAccessToken(accessToken);
                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Taskify-GoogleMeetIntegration",
                });
                var emails = users.Where(user => obj.MeetingUserIds.Contains(user.Id))
                          .Select(user => user.Email)
                          .ToList();

                var attendees = new List<EventAttendee>();
                foreach (var email in emails)
                {
                    attendees.Add(new EventAttendee { Email = email });
                }
                var newEvent = new Event()
                {
                    Summary = obj.Title,
                    Start = new EventDateTime()
                    {
                        DateTime = obj.StartDateTime,
                        TimeZone = SD.TimeZone,
                    },
                    End = new EventDateTime()
                    {
                        DateTime = obj.EndDateTime,
                        TimeZone = SD.TimeZone,
                    },
                    Attendees = attendees,
                    ConferenceData = new ConferenceData()
                    {
                        CreateRequest = new CreateConferenceRequest()
                        {
                            RequestId = Guid.NewGuid().ToString(),
                        },
                    },
                };
                var request = service.Events.Insert(newEvent, "primary");
                request.ConferenceDataVersion = 1;
                obj.RequestId = newEvent.ConferenceData.CreateRequest.RequestId;

                var createdEvent = await request.ExecuteAsync();
                obj.MeetingUrl = createdEvent.ConferenceData.EntryPoints[0].Uri;
                APIResponse result = await _meetingService.CreateAsync<APIResponse>(obj);

                if (result != null && result.IsSuccess && result.ErrorMessages.Count == 0)
                {
                    var project = JsonConvert.DeserializeObject<ProjectDTO>(Convert.ToString(result.Result));
                    if (obj.MeetingUserIds != null && obj.MeetingUserIds.Count > 0)
                        await AddUserToMeeting(obj.MeetingUserIds, project.Id);

                    TempData["success"] = "Create new meeting successfully";
                    return RedirectToAction("Index", "Meeting");
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
                    identity.AddClaim(new Claim("access_token_google", token));
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                    {
                        IsPersistent = true, // Persistent cookie
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Cookie expiration set to 30 minutes
                    });
                    _tokenProvider.SetToken(model);
                    if (!string.IsNullOrEmpty(token))
                    {
                        TempData["success"] = "Login google successful!";
                    }
                    else
                    {
                        TempData["error"] = "Login google failed!";
                    }
                    return RedirectToAction("Create", "Meeting");

                }
                else
                {
                    TempData["error"] = response.ErrorMessages.FirstOrDefault();
                    return RedirectToAction("Create", "Meeting");
                }
            }
            else
            {
                TempData["error"] = "Token is null";
                return RedirectToAction("Create", "Meeting");
            }
        }



        private async Task<List<UserDTO>> GetUsersByWorkspaceIdAsync(string userId, int workspaceId)
        {
            var response = await _workspaceUserService.GetByWorkspaceIdAsync<APIResponse>(workspaceId);
            List<WorkspaceUserDTO> workspaceUsers = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                workspaceUsers = JsonConvert.DeserializeObject<List<WorkspaceUserDTO>>(Convert.ToString(response.Result));
            }
            var users = new List<UserDTO>();
            if (workspaceUsers.Count > 0)
            {
                users = workspaceUsers.Select(x => x.User).Where(x => !x.Id.Equals(userId)).ToList();

            }
            if (users.Count == 0)
                TempData["warning"] = "No users are available.";
            return users;
        }

        private async Task<bool> AddUserToMeeting(List<string> userIds, int meetingId)
        {
            try
            {
                foreach (var user in userIds)
                {
                    var obj = new MeetingUserDTO { UserId = user, MeetingId = meetingId };
                    var response = await _meetingUserService.CreateAsync<APIResponse>(obj);
                    if (response == null && response.IsSuccess && response.ErrorMessages.Count == 0)
                    {
                        TempData["error"] = response.ErrorMessages.FirstOrDefault();
                        return false;
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Internal Server Error! {ex.Message}";
                return false;
            }
        }

        private async Task<MeetingDTO> GetMeetingByIdAsync(int id)
        {
            var response = await _meetingService.GetAsync<APIResponse>(id);
            MeetingDTO obj = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                obj = JsonConvert.DeserializeObject<MeetingDTO>(Convert.ToString(response.Result));
            }
            return obj;
        }

        private async Task<List<MeetingDTO>> GetMeetingByWorkspaceIdAsync(int id)
        {
            var response = await _meetingService.GetByWorkspaceIdAsync<APIResponse>(id);
            List<MeetingDTO> list = new();
            if (response != null && response.IsSuccess && response.ErrorMessages.Count == 0)
            {
                list = JsonConvert.DeserializeObject<List<MeetingDTO>>(Convert.ToString(response.Result));
            }
            if (list.Count > 0)
            {
                list = list.OrderByDescending(x => x.StartDateTime)
                   .ThenByDescending(x => x.EndDateTime)
                   .ToList();
            }
            return list;
        }

    }
}
