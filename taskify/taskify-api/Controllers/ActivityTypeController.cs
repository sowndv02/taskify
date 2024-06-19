using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using taskify_api.Models;
using taskify_api.Models.DTO;
using taskify_api.Repository.IRepository;

namespace taskify_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityTypeController : ControllerBase
    {
        private readonly IActivityTypeRepository _activityTypeRepository;
        protected APIResponse _response;
        private readonly ILogger<ColorController> _logger;
        private readonly IMapper _mapper;
        public ActivityTypeController(IActivityTypeRepository activityTypeRepository, ILogger<ColorController> logger, IMapper mapper)
        {
            _activityTypeRepository = activityTypeRepository;
            _logger = logger;
            _mapper = mapper;
            _response = new();
        }


        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetAllAsync()
        {
            try
            {
                List<ActivityType> list = await _activityTypeRepository.GetAllAsync();
                _response.Result = _mapper.Map<List<ActivityTypeDTO>>(list);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }

        [HttpGet("{code}", Name = "GetByTitle")]
        public async Task<ActionResult<APIResponse>> GetByTitleAsync(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string> { $"{code} is null or empty!" };
                    return BadRequest(_response);
                }
                List<ActivityType> model = await _activityTypeRepository.GetAllAsync(x => x.Title.Equals(code));
                _response.Result = _mapper.Map<List<ActivityTypeDTO>>(model);
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }


        [HttpGet("{id:int}", Name = "GetById")]
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
                ActivityType model = await _activityTypeRepository.GetAsync(x => x.Id == id);
                _response.Result = _mapper.Map<ActivityTypeDTO>(model);
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }

        [HttpPost]
        public async Task<ActionResult<APIResponse>> CreateAsync([FromBody] ActivityTypeDTO createDTO)
        {
            try
            {

                if (createDTO == null) return BadRequest(createDTO);
                ActivityType model = _mapper.Map<ActivityType>(createDTO);
                await _activityTypeRepository.CreateAsync(model);
                _response.Result = _mapper.Map<ColorDTO>(model);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetById", new { model.Id }, _response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }



        [HttpPut("{id:int}")]
        public async Task<ActionResult<APIResponse>> UpdateAsync(int id, [FromBody] ActivityTypeDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.Id)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "Color Id invalid!" };
                    return BadRequest(_response);
                }
                ActivityType model = _mapper.Map<ActivityType>(updateDTO);
                await _activityTypeRepository.UpdateAsync(model);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }
    }
}
