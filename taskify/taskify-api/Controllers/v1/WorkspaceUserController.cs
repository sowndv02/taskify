﻿using AutoMapper;
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
    public class WorkspaceUserController : ControllerBase
    {
        private readonly IWorkspaceUserRepository _workspaceUserRepository;
        protected APIResponse _response;
        private readonly IMapper _mapper;
        public WorkspaceUserController(IWorkspaceUserRepository workspaceUserRepository, IMapper mapper)
        {
            _workspaceUserRepository = workspaceUserRepository;
            _mapper = mapper;
            _response = new();
        }


        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetAllAsync()
        {
            try
            {
                List<WorkspaceUser> list = await _workspaceUserRepository.GetAllAsync();
                _response.Result = _mapper.Map<List<WorkspaceUserDTO>>(list);
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

        [HttpGet("user/{id}", Name = "GetWorkspaceUserByUserId")]
        public async Task<ActionResult<APIResponse>> GetAsyncByUserId(string id)
        {
            try
            {
                List<WorkspaceUser> list = await _workspaceUserRepository.GetAllAsync(x => x.UserId.Equals(id));
                _response.Result = _mapper.Map<List<WorkspaceUserDTO>>(list);
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


        [HttpGet("{id:int}", Name = "GetWorkspaceUserByWorkspaceId")]
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
                List<WorkspaceUser> model = await _workspaceUserRepository.GetAllAsync(x => x.WorkspaceId == id, "User");
                _response.Result = _mapper.Map<List<WorkspaceUserDTO>>(model);
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
        public async Task<ActionResult<APIResponse>> CreateAsync([FromBody] WorkspaceUserDTO createDTO)
        {
            try
            {

                if (createDTO == null) return BadRequest(createDTO);
                WorkspaceUser model = _mapper.Map<WorkspaceUser>(createDTO);
                await _workspaceUserRepository.CreateAsync(model);
                _response.Result = _mapper.Map<WorkspaceUserDTO>(model);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetWorkspaceUserById", new { model.Id }, _response);
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
        public async Task<ActionResult<APIResponse>> UpdateAsync(int id, [FromBody] WorkspaceUserDTO updateDTO)
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
                WorkspaceUser model = _mapper.Map<WorkspaceUser>(updateDTO);
                await _workspaceUserRepository.UpdateAsync(model);
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
                var obj = await _workspaceUserRepository.GetAsync(x => x.Id == id);

                if (obj == null) return NotFound();

                await _workspaceUserRepository.RemoveAsync(obj);
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

        [HttpDelete("{workspaceId}/{userId}", Name = "DeleteWorkspaceUserByWorkspaceAndUser")]
        public async Task<ActionResult<APIResponse>> DeleteProjectUserAsync(int workspaceId, string userId)
        {
            try
            {
                if (workspaceId == 0 || string.IsNullOrEmpty(userId)) return BadRequest();
                var obj = await _workspaceUserRepository.GetAsync(x => x.WorkspaceId == workspaceId && x.UserId.Equals(userId));

                if (obj == null) return NotFound();

                await _workspaceUserRepository.RemoveAsync(obj);
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
