﻿using AutoMapper;
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
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IColorRepository _colorRepository;
        protected APIResponse _response;
        private readonly IMapper _mapper;
        public ProjectController(IProjectRepository projectRepository, IMapper mapper, IColorRepository colorRepository)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
            _response = new();
            _colorRepository = colorRepository;
        }


        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetAllAsync()
        {
            try
            {
                List<Project> list = await _projectRepository.GetAllAsync(null, "Status, ProjectTags");
                _response.Result = _mapper.Map<List<ProjectDTO>>(list);
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

        [HttpGet("{id:int}", Name = "GetProjectById")]
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
                Project model = await _projectRepository.GetAsync(x => x.Id == id);
                _response.Result = _mapper.Map<ProjectDTO>(model);
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

        [HttpGet("workspace/{id:int}", Name = "GetProjectByWorkspaceId")]
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
                List<Project> model = await _projectRepository.GetAllAsync(x => x.WorkspaceId == id);
                _response.Result = _mapper.Map<List<ProjectDTO>>(model);
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


        [HttpGet("{key}", Name = "GetProjectByTitle")]
        public async Task<ActionResult<APIResponse>> GetProjectByTitleAsync(string key)
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
                List<Project> model = await _projectRepository.GetAllAsync(x => x.Title.Contains(key));
                _response.Result = _mapper.Map<List<ProjectDTO>>(model);
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
        public async Task<ActionResult<APIResponse>> CreateAsync([FromBody] ProjectDTO createDTO)
        {
            try
            {

                if (createDTO == null) return BadRequest(createDTO);
                Project model = _mapper.Map<Project>(createDTO);
                await _projectRepository.CreateAsync(model);
                _response.Result = _mapper.Map<ProjectDTO>(model);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetProjectById", new { model.Id }, _response);
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
        public async Task<ActionResult<APIResponse>> UpdateAsync(int id, [FromBody] ProjectDTO updateDTO)
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
                Project model = _mapper.Map<Project>(updateDTO);
                await _projectRepository.UpdateAsync(model);
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
                var obj = await _projectRepository.GetAsync(x => x.Id == id);

                if (obj == null) return NotFound();

                await _projectRepository.RemoveAsync(obj);
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


        [HttpGet("{userId}/{workspaceId:int}", Name = "GetProjectByUserIdAndWorkspaceId")]
        public async Task<ActionResult<APIResponse>> GetAsyncByUserIdAndWorkspaceId(string userId, int workspaceId)
        {
            try
            {
                List<Project> list = await _projectRepository.GetAllAsync(x => x.OwnerId.Equals(userId) && x.WorkspaceId == workspaceId, "Status");
                foreach (var project in list)
                {
                    if (project.Status != null)
                    {
                        project.Status.Color = await _colorRepository.GetAsync(x => x.Id == project.Status.ColorId);
                    }
                }

                _response.Result = _mapper.Map<List<ProjectDTO>>(list);
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

        [HttpGet("status/{id:int}", Name = "GetProjectByStatusId")]
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
                List<Project> model = await _projectRepository.GetAllAsync(x => x.StatusId == id);
                _response.Result = _mapper.Map<ProjectDTO>(model);
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
