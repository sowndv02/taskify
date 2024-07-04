using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Security.AccessControl;
using taskify_api.Models;
using taskify_api.Models.DTO;
using taskify_api.Repository.IRepository;
using taskify_utility;

namespace taskify_api.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ProjectMediaController : ControllerBase
    {

        private readonly IProjectMediaRepository _projectMediaRepository;
        protected APIResponse _response;
        private readonly IMapper _mapper;
        public ProjectMediaController(IProjectMediaRepository projectMediaRepository, IMapper mapper)
        {
            _projectMediaRepository = projectMediaRepository;
            _mapper = mapper;
            _response = new();
        }


        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetAllAsync()
        {
            try
            {
                List<ProjectMedia> list = await _projectMediaRepository.GetAllAsync();
                _response.Result = _mapper.Map<List<ProjectMediaDTO>>(list);
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

        [HttpGet("{id:int}", Name = "GetProjectMediaById")]
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
                ProjectMedia model = await _projectMediaRepository.GetAsync(x => x.Id == id);
                _response.Result = _mapper.Map<ProjectMediaDTO>(model);
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
        public async Task<ActionResult<APIResponse>> CreateAsync([FromForm] ProjectMediaDTO createDTO)
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
                ProjectMedia model = _mapper.Map<ProjectMedia>(createDTO);
                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                if (createDTO.File != null && createDTO.File.Length > 0)
                {


                    string fileName = createDTO.FileName;
                    string filePath = @"wwwroot\ProjectMedia\" + createDTO.ProjectId + @"\" + fileName;
                    var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                    using (var stream = new FileStream(directoryLocation, FileMode.Create))
                    {
                        createDTO.File.CopyTo(stream);
                    }

                    model.MediaLocalPathUrl = filePath;
                    model.MediaUrl = baseUrl + $"/ProjectMedia/" + createDTO.ProjectId + "/" + fileName;
                    
                    await _projectMediaRepository.CreateAsync(model);
                    _response.Result = _mapper.Map<ProjectMediaDTO>(model);
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
        public async Task<ActionResult<APIResponse>> UpdateAsync(int id, [FromBody] ProjectMediaDTO updateDTO)
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
                ProjectMedia model = _mapper.Map<ProjectMedia>(updateDTO);
                await _projectMediaRepository.UpdateAsync(model);
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
                var obj = await _projectMediaRepository.GetAsync(x => x.Id == id);

                if (obj == null) return NotFound();

                await _projectMediaRepository.RemoveAsync(obj);
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



        [HttpGet("project/{id:int}", Name = "GetProjectMediaByProjectId")]
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
                List<ProjectMedia> model = await _projectMediaRepository.GetAllAsync(x => x.ProjectId == id);
                _response.Result = _mapper.Map<List<ProjectMediaDTO>>(model);
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
