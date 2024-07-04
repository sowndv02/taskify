using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using taskify_api.Models;
using taskify_api.Models.DTO;
using taskify_api.Repository.IRepository;
using taskify_utility;

namespace taskify_api.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        protected APIResponse _response;
        private readonly IMapper _mapper;
        public UserController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _response = new();
        }


        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetAllAsync()
        {
            try
            {
                List<User> list = await _userRepository.GetAllAsync();
                _response.Result = _mapper.Map<List<UserDTO>>(list);
                _response.StatusCode = HttpStatusCode.OK;
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

        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<ActionResult<APIResponse>> GetByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string> { $"{id} is invalid!" };
                    return BadRequest(_response);
                }
                User model = await _userRepository.GetAsync(id);
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

        [HttpPut("password/{id}", Name = "UpdatePassword")]
        public async Task<ActionResult<APIResponse>> GetPasswordByIdAsync(string id, [FromBody] UpdatePasswordRequestDTO updatePasswordRequestDTO)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string> { $"{id} is invalid!" };
                    return BadRequest(_response);
                }
                User model = await _userRepository.UpdatePasswordAsync(updatePasswordRequestDTO);
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

        [HttpPost]
        public async Task<ActionResult<APIResponse>> CreateAsync([FromBody] UserDTO createDTO)
        {
            try
            {

                if (createDTO == null) return BadRequest(createDTO);
                User model = _mapper.Map<User>(createDTO);
                await _userRepository.CreateAsync(model, "");
                _response.Result = _mapper.Map<UserDTO>(model);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetUserById", new { model.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }



        [HttpPut("{id}")]
        public async Task<ActionResult<APIResponse>> UpdateAsync(string id, [FromForm] UserDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || !id.Equals(updateDTO.Id))
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "Id invalid!" };
                    return BadRequest(_response);
                }
                var model = await _userRepository.GetAsync(updateDTO.Id);
                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                if (updateDTO.Image != null)
                {
                    if (!string.IsNullOrEmpty(model.ImageLocalPathUrl))
                    {
                        var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), model.ImageLocalPathUrl);
                        FileInfo file = new FileInfo(oldFilePathDirectory);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }
                    string fileName = updateDTO.Id + Path.GetExtension(updateDTO.Image.FileName);
                    string filePath = @"wwwroot\UserImage\" + fileName;

                    var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                    using (var fileStream = new FileStream(directoryLocation, FileMode.Create))
                    {
                        updateDTO.Image.CopyTo(fileStream);
                    }
                    model.ImageUrl = baseUrl + $"/{SD.UrlImageUser}/" + fileName;
                    model.ImageLocalPathUrl = filePath;
                }
                model.PhoneNumber = updateDTO.PhoneNumber;
                model.Address = updateDTO.Address;
                model.FirstName = updateDTO.FirstName;  
                model.LastName = updateDTO.LastName;    

                await _userRepository.UpdateAsync(model);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
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


        [HttpPut("UploadImg/{id}")]
        public async Task<ActionResult<APIResponse>> UploadImgAsync(string id, [FromForm] UserDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || !id.Equals(updateDTO.Id))
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "Id invalid!" };
                    return BadRequest(_response);
                }
                User model = await _userRepository.GetAsync(id);

                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                if (updateDTO.Image != null)
                {
                    if (!string.IsNullOrEmpty(model.ImageLocalPathUrl))
                    {
                        var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), model.ImageLocalPathUrl);
                        FileInfo file = new FileInfo(oldFilePathDirectory);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }
                    string fileName = updateDTO.Id + Path.GetExtension(updateDTO.Image.FileName);
                    string filePath = @"wwwroot\UserImage\" + fileName;

                    var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                    using (var fileStream = new FileStream(directoryLocation, FileMode.Create))
                    {
                        updateDTO.Image.CopyTo(fileStream);
                    }


                    
                    model.ImageUrl = baseUrl + $"/{SD.UrlImageUser}/" + fileName;
                    model.ImageLocalPathUrl = filePath;
                }
                else
                {
                    model.ImageUrl = baseUrl + $"/{SD.UrlImageUser}/" + SD.UrlImageDefault;
                    model.ImageLocalPathUrl = @"wwwroot\UserImage\" + SD.UrlImageDefault;
                }

                await _userRepository.UpdateAsync(model);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
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


        [HttpDelete("{id}")]
        public async Task<ActionResult<APIResponse>> DeleteAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) return BadRequest();
                var obj = await _userRepository.GetAsync(id);

                if (obj == null) return NotFound();

                await _userRepository.RemoveAsync(id);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }
    }
}
