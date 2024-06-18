using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using taskify_api.Models;
using taskify_api.Models.DTO;
using taskify_api.Repository.IRepository;

namespace taskify_api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;
        protected APIResponse _response;
        private readonly ILogger<RoleController> _logger;
        private readonly IMapper _mapper;

        public RoleController(IRoleRepository roleRepository, IMapper mapper, ILogger<RoleController> logger)
        {
            _response = new();
            _logger = logger;
            _mapper = mapper;
            _roleRepository = roleRepository;
        }

        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetRolesAsync()
        {
            try
            {
                List<IdentityRole> list = await _roleRepository.GetAllAsync();
                _response.Result = _mapper.Map<List<RoleDTO>>(list);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<APIResponse>> GetRoleByName(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string> { $"{name} is null or empty!" };
                    return BadRequest(_response);
                }
                IdentityRole model = await _roleRepository.GetByNameAsync(name);
                _response.Result = _mapper.Map<RoleDTO>(model);
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPost]
        public async Task<ActionResult<APIResponse>> CreateRoleAsync(RoleDTO roleDTO)
        {
            try
            {
                if(await _roleRepository.GetByNameAsync(roleDTO.Name) != null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string> { $"{roleDTO.Name} existed!" };
                    return BadRequest(_response);
                }
                if (roleDTO == null) return BadRequest(roleDTO);
                IdentityRole model = _mapper.Map<IdentityRole>(roleDTO);
                await _roleRepository.CreateAsync(model);
                _response.Result = _mapper.Map<RoleDTO>(model);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetRoleByName", new { model.Name }, _response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
    }
}
