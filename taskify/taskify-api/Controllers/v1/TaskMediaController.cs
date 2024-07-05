using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using taskify_api.Models.DTO;
using taskify_api.Models;
using taskify_api.Repository.IRepository;
using taskify_api.Repository;

namespace taskify_api.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class TaskMediaController : ControllerBase
    {
        private readonly ITaskMediaRepository _taskMediaRepository;
        protected APIResponse _response;
        private readonly IMapper _mapper;
        public TaskMediaController(ITaskMediaRepository taskMediaRepository, IMapper mapper)
        {
            _taskMediaRepository = taskMediaRepository;
            _mapper = mapper;
            _response = new();
        }


        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetAllAsync()
        {
            try
            {
                List<TaskMedia> list = await _taskMediaRepository.GetAllAsync();
                _response.Result = _mapper.Map<List<TaskMediaDTO>>(list);
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

        [HttpGet("{id:int}", Name = "GetTaskMediaById")]
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
                TaskMedia model = await _taskMediaRepository.GetAsync(x => x.Id == id);
                _response.Result = _mapper.Map<TaskMediaDTO>(model);
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
        public async Task<ActionResult<APIResponse>> CreateAsync([FromForm] TaskMediaDTO createDTO)
        {
            try
            {

                if (createDTO == null || createDTO.File?.Length == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "File invalid!" };
                    return BadRequest(_response);
                }
                TaskMedia model = _mapper.Map<TaskMedia>(createDTO);
                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                if (createDTO.File != null && createDTO.File.Length > 0)
                {
                    string fileName = createDTO.FileName;
                    string filePath = @"wwwroot\TaskMedia\" + createDTO.TaskId + @"\" + fileName;
                    var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                    using (var stream = new FileStream(directoryLocation, FileMode.Create))
                    {
                        createDTO.File.CopyTo(stream);
                    }

                    model.MediaLocalPathUrl = filePath;
                    model.MediaUrl = baseUrl + $"/TaskMedia/" + createDTO.TaskId + "/" + fileName;
                    model.CreatedDate = DateTime.Now;
                    await _taskMediaRepository.CreateAsync(model);
                    _response.Result = _mapper.Map<TaskMediaDTO>(model);
                    _response.StatusCode = HttpStatusCode.Created;
                    return Ok(_response);

                }
                else
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "File invalid!" };
                    return BadRequest(_response);
                }


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
        public async Task<ActionResult<APIResponse>> UpdateAsync(int id, [FromBody] TaskMediaDTO updateDTO)
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
                TaskMedia model = _mapper.Map<TaskMedia>(updateDTO);
                await _taskMediaRepository.UpdateAsync(model);
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
                var obj = await _taskMediaRepository.GetAsync(x => x.Id == id);

                if (obj == null) return NotFound();

                await _taskMediaRepository.RemoveAsync(obj);
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



        [HttpGet("task/{id:int}", Name = "GetTaskMediaByTaskId")]
        public async Task<ActionResult<APIResponse>> GetByTaskIdAsync(int id)
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
                List<TaskMedia> model = await _taskMediaRepository.GetAllAsync(x => x.TaskId == id);
                _response.Result = _mapper.Map<List<TaskMediaDTO>>(model);
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
