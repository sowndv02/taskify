using AutoMapper;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using taskify_api.Models;
using taskify_api.Models.DTO;
using taskify_api.Repository.IRepository;
using taskify_api.Utils;
using taskify_utility;

namespace taskify_api.Controllers
{
    [Route("api/v{version:apiVersion}/UserAuth")]
    [ApiController]
    [ApiVersionNeutral]
    public class UserAuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        protected APIResponse _response;
        private readonly IMapper _mapper;
        private string audience = string.Empty;
        public UserAuthController(IUserRepository userRepository, IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _response = new();
            audience = configuration.GetValue<string>("Authentication:Google:ClientId");
        }


        [HttpGet("Error")]
        public async Task<IActionResult> Error()
        {
            throw new FileNotFoundException();
        }

        [HttpGet("ImageError")]
        public async Task<IActionResult> ImageError()
        {
            throw new BadImageFormatException("Fake image exception.");
        }

        private async Task<GoogleJsonWebSignature.Payload> ValidateGoogleToken(string token)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(token, new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>()
                });

                return payload;
            }
            catch
            {
                return null;
            }
        }

        [HttpPost("google")]
        public async Task<IActionResult> Google([FromBody] GoogleAuthDTO googleAuthDTO)
        {
            try
            {
                //var payload = await ValidateGoogleToken(googleAuthDTO.Token);
                //if (payload == null)
                //{
                //    _response.StatusCode = HttpStatusCode.BadRequest;
                //    _response.IsSuccess = false;
                //    _response.ErrorMessages = new List<string> { $"token is invalid!" };
                //    return BadRequest(_response);
                //}

                var user = await _userRepository.GetUserByEmailAsync(googleAuthDTO.Email);
                if (user == null)
                {
                    
                    user = new User
                    {
                        UserName = googleAuthDTO.Email,
                        FirstName = googleAuthDTO.FirstName,
                        LastName = googleAuthDTO.LastName,
                        PasswordHash = PasswordGenerator.GeneratePassword(length: 12, requireDigit: true, requireLowercase: true, requireUppercase: true, requireNonAlphanumeric: true),
                        Email = googleAuthDTO.Email,
                        NormalizedEmail = googleAuthDTO.Email
                    };
                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    user.ImageUrl = baseUrl + $"/{SD.UrlImageUser}/" + SD.UrlImageAvatarDefault;
                    user.ImageLocalPathUrl = @"wwwroot\UserImage\" + SD.UrlImageAvatarDefault;
                    await _userRepository.CreateAsync(user, user.PasswordHash);
                }
                var tokenDto = await _userRepository.Login(new LoginRequestDTO() { UserName = user.Email, Password = user.PasswordHash }, false);
                if (tokenDto == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("User is currently locked out.");
                    return BadRequest(_response);
                }
                if (string.IsNullOrEmpty(tokenDto.AccessToken))
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Username or password is incorrect");
                    return BadRequest(_response);
                }
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = tokenDto;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserByIdAsync(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string> { $"{userId} is invalid!" };
                    return BadRequest(_response);
                }
                User model = await _userRepository.GetAsync(userId);
                _response.Result = _mapper.Map<UserDTO>(model);
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO model)
        {
            var tokenDto = await _userRepository.Login(model);
            if (tokenDto == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("User is currently locked out.");
                return BadRequest(_response);
            }
            if (string.IsNullOrEmpty(tokenDto.AccessToken))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username or password is incorrect");
                return BadRequest(_response);
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = tokenDto;
            return Ok(_response);
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterationRequestDTO model)
        {
            bool ifUserNameUnique = _userRepository.IsUniqueUser(model.UserName);
            if (!ifUserNameUnique)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username already exists");
                return BadRequest(_response);
            }

            var user = await _userRepository.Register(model);
            if (user == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Error while registering");
                return BadRequest(_response);
            }
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
            user.ImageUrl = baseUrl + $"/{SD.UrlImageUser}/" + SD.UrlImageAvatarDefault;
            user.ImageLocalPathUrl = @"wwwroot\UserImage\" + SD.UrlImageAvatarDefault;
            await _userRepository.UpdateAsync(user);
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return Ok(_response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> GetNewTokenFromRefreshToken([FromBody] TokenDTO model)
        {
            if (ModelState.IsValid)
            {
                var tokenDTOResponse = await _userRepository.RefreshAccessToken(model);
                if (tokenDTOResponse == null || string.IsNullOrEmpty(tokenDTOResponse.AccessToken))
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Token Invalid");
                    return BadRequest(_response);
                }
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = tokenDTOResponse;
                return Ok(_response);
            }
            else
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("Error while refresh token");
                return BadRequest(_response);
            }
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] TokenDTO model)
        {
            if (ModelState.IsValid)
            {
                await _userRepository.RevokeRefreshToken(model);
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            _response.IsSuccess = false;
            _response.Result = "Invalid Input";
            return BadRequest(_response);

        }

    }
}


