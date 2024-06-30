using AutoMapper;
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
    public class ColorController : ControllerBase
    {
        private readonly IColorRepository _colorRepository;
        protected APIResponse _response;
        private readonly IMapper _mapper;
        public ColorController(IColorRepository colorRepository, IMapper mapper)
        {
            _colorRepository = colorRepository;
            _mapper = mapper;
            _response = new();
        }


        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetAllColorsAsync()
        {
            try
            {
                List<Color> list = await _colorRepository.GetAllAsync();
                _response.Result = _mapper.Map<List<ColorDTO>>(list);
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

        [HttpGet("search/{code}", Name = "GetColorByCode")]
        public async Task<ActionResult<APIResponse>> GetColorByCodeAsync(string code)
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
                List<Color> model = await _colorRepository.GetAllAsync(x => x.ColorCode.Equals(code));
                _response.Result = _mapper.Map<List<ColorDTO>>(model);
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


        [HttpGet("{userId}", Name = "GetColorByUserId")]
        public async Task<ActionResult<APIResponse>> GetAsyncByUserId(string userId)
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
                List<Color> list = await _colorRepository.GetAllAsync(x => x.UserId.Equals(userId) || x.IsDefault);
                _response.Result = _mapper.Map<List<ColorDTO>>(list);
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


        [HttpGet("{id:int}", Name = "GetColorById")]
        public async Task<ActionResult<APIResponse>> GetColorByIdAsync(int id)
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
                Color model = await _colorRepository.GetAsync(x => x.Id == id);
                _response.Result = _mapper.Map<ColorDTO>(model);
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
        public async Task<ActionResult<APIResponse>> CreateColorAsync([FromBody] ColorDTO colorDTO)
        {
            try
            {

                if (colorDTO == null) return BadRequest(colorDTO);
                Color model = _mapper.Map<Color>(colorDTO);
                await _colorRepository.CreateAsync(model);
                _response.Result = _mapper.Map<ColorDTO>(model);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetColorById", new { model.Id }, _response);
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
        public async Task<ActionResult<APIResponse>> UpdateColorAsync(int id, [FromBody] ColorDTO colorDTO)
        {
            try
            {
                if (colorDTO == null || id != colorDTO.Id)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "Color Id invalid!" };
                    return BadRequest(_response);
                }
                Color model = _mapper.Map<Color>(colorDTO);
                await _colorRepository.UpdateAsync(model);
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
        public async Task<ActionResult<APIResponse>> DeleteColor(int id)
        {
            try
            {
                if (id == 0) return BadRequest();
                var color = await _colorRepository.GetAsync(x => x.Id == id);

                if (color == null) return NotFound();

                //if (!string.IsNullOrEmpty(villa.ImageLocalPathUrl))
                //{
                //    var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), villa.ImageLocalPathUrl);
                //    FileInfo file = new FileInfo(oldFilePathDirectory);
                //    if (file.Exists)
                //    {
                //        file.Delete();
                //    }
                //}

                await _colorRepository.RemoveAsync(color);
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
