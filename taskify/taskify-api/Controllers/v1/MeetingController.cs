using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using taskify_api.Models;
using taskify_api.Models.DTO;
using taskify_api.Repository.IRepository;

namespace taskify_api.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class MeetingController : ControllerBase
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly IMeetingUserRepository _meetingUserRepository;
        private readonly IUserRepository _userRepository;
        protected APIResponse _response;
        private readonly IMapper _mapper;
        public MeetingController(IMeetingRepository meetingRepository, IMapper mapper,
            IMeetingUserRepository meetingUserRepository, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _meetingRepository = meetingRepository;
            _mapper = mapper;
            _response = new();
            _meetingUserRepository = meetingUserRepository;
            _userRepository = userRepository;
        }


        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetAllAsync()
        {
            try
            {
                List<Meeting> list = await _meetingRepository.GetAllAsync();
                _response.Result = _mapper.Map<List<MeetingDTO>>(list);
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

        [HttpGet("{id:int}", Name = "GetMeetingById")]
        public async Task<ActionResult<APIResponse>> GetByIdAsync(int id)
        {
            try
            {
                if (id < 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string> { $"{id} is invalid!" };
                    return BadRequest(_response);
                }
                Meeting model = await _meetingRepository.GetAsync(x => x.Id == id, true, "MeetingUsers");
                _response.Result = _mapper.Map<MeetingDTO>(model);
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

        [HttpGet("workspace/{id:int}", Name = "GetMeetingByWorkspaceId")]
        public async Task<ActionResult<APIResponse>> GetByWorkspaceIdAsync(int id)
        {
            try
            {
                if (id < 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string> { $"{id} is invalid!" };
                    return BadRequest(_response);
                }
                List<Meeting> model = await _meetingRepository.GetAllAsync(x => x.WorkspaceId == id);
                foreach (var item in model)
                {
                    item.MeetingUsers = await _meetingUserRepository.GetAllAsync(x => x.MeetingId == item.Id);
                    foreach (var user in item.MeetingUsers)
                    {
                        user.User = await _userRepository.GetAsync(user.UserId);
                    }
                }
                _response.Result = _mapper.Map<List<MeetingDTO>>(model);
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


        [HttpGet("user/{userId}", Name = "GetMeetingByUserId")]
        public async Task<ActionResult<APIResponse>> GetMeetingByUserIdAsync(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string> { $"{userId} is null or empty!" };
                    return BadRequest(_response);
                }
                List<Meeting> model = await _meetingRepository.GetAllAsync();
                _response.Result = _mapper.Map<List<MeetingDTO>>(model);
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
        public async Task<ActionResult<APIResponse>> CreateAsync([FromBody] MeetingDTO createDTO)
        {
            try
            {
                if (createDTO == null) return BadRequest(createDTO);
                Meeting model = _mapper.Map<Meeting>(createDTO);
                await _meetingRepository.CreateAsync(model);
                _response.Result = _mapper.Map<MeetingDTO>(model);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetMeetingById", new { model.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }



        [HttpPut("{id:int}")]
        public async Task<ActionResult<APIResponse>> UpdateAsync(int id, [FromBody] MeetingDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.Id)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "Id invalid!" };
                    return BadRequest(_response);
                }
                Meeting model = _mapper.Map<Meeting>(updateDTO);
                await _meetingRepository.UpdateAsync(model);
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
        public async Task<ActionResult<APIResponse>> DeleteAsync(int id)
        {
            try
            {
                if (id == 0) return BadRequest();
                var obj = await _meetingRepository.GetAsync(x => x.Id == id);

                if (obj == null) return NotFound();

                await _meetingRepository.RemoveAsync(obj);
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


        [HttpGet("{userId}/{workspaceId:int}", Name = "GetMeetingByUserIdAndWorkspaceId")]
        public async Task<ActionResult<APIResponse>> GetAsyncByUserIdAndWorkspaceId(string userId, int workspaceId)
        {
            try
            {
                List<Meeting> list = await _meetingRepository.GetAllAsync(x => x.OwnerId.Equals(userId) && x.WorkspaceId == workspaceId, "MeetingUser");
                _response.Result = _mapper.Map<List<MeetingDTO>>(list);
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
    }
}
