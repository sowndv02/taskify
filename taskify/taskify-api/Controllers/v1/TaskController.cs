using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using taskify_api.Models;
using taskify_api.Models.DTO;
using taskify_api.Repository;
using taskify_api.Repository.IRepository;

namespace taskify_api.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;
        protected APIResponse _response;
        private readonly IMapper _mapper;
        public TaskController(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
            _response = new();
        }


        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetAllAsync()
        {
            try
            {
                List<TaskModel> list = await _taskRepository.GetAllAsync();
                _response.Result = _mapper.Map<List<TaskDTO>>(list);
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

        [HttpGet("{id:int}", Name = "GetTaskById")]
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
                TaskModel model = await _taskRepository.GetAsync(x => x.Id == id);
                _response.Result = _mapper.Map<TaskDTO>(model);
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

        [HttpGet("{key}", Name = "GetTaskByTitle")]
        public async Task<ActionResult<APIResponse>> GetTaskByTitleAsync(string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string> { $"{key} is null or empty!" };
                    return BadRequest(_response);
                }
                List<TaskModel> model = await _taskRepository.GetAllAsync(x => x.Title.Contains(key));
                _response.Result = _mapper.Map<List<TaskDTO>>(model);
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
        public async Task<ActionResult<APIResponse>> CreateAsync([FromBody] TaskDTO createDTO)
        {
            try
            {

                if (createDTO == null) return BadRequest(createDTO);
                TaskModel model = _mapper.Map<TaskModel>(createDTO);
                await _taskRepository.CreateAsync(model);
                _response.Result = _mapper.Map<TaskDTO>(model);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetTaskById", new { model.Id }, _response);
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
        public async Task<ActionResult<APIResponse>> UpdateAsync(int id, [FromBody] TaskDTO updateDTO)
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
                TaskModel model = _mapper.Map<TaskModel>(updateDTO);
                await _taskRepository.UpdateAsync(model);
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
                var obj = await _taskRepository.GetAsync(x => x.Id == id);

                if (obj == null) return NotFound();

                await _taskRepository.RemoveAsync(obj);
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


        [HttpGet("status/{id:int}", Name = "GetTaskByStatusId")]
        public async Task<ActionResult<APIResponse>> GetByStatusIdAsync(int id)
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
                List<TaskModel> model = await _taskRepository.GetAllAsync(x => x.StatusId == id);
                _response.Result = _mapper.Map<TaskDTO>(model);
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

        [HttpGet("project/{id:int}", Name = "GetTaskByProjectId")]
        public async Task<ActionResult<APIResponse>> GetByProjectIdAsync(int id)
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
                List<TaskModel> model = await _taskRepository.GetAllAsync(x => x.ProjectId == id);
                _response.Result = _mapper.Map<List<TaskDTO>>(model);
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

        [HttpGet("project/{id:int}/{statusId:int}", Name = "GetTaskByProjectIdAndStatusId")]
        public async Task<ActionResult<APIResponse>> GetByProjectIdAndStatusIdAsync(int id, int statusId)
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
                List<TaskModel> model = await _taskRepository.GetAllAsync(x => x.ProjectId == id && x.StatusId == statusId);
                _response.Result = _mapper.Map<List<TaskDTO>>(model);
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
