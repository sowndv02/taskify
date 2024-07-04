using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
using taskify_font_end.Service.IService;
using taskify_utility;

namespace taskify_font_end.Service
{
    public class UserService : IUserService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string API_URL;
        private readonly IBaseServices _baseServices;

        public UserService(IHttpClientFactory clientFactory, IConfiguration configuration, IBaseServices baseServices)
        {
            _baseServices = baseServices;
            API_URL = configuration.GetValue<string>("ServiceUrls:TaskifyAPI");
            _clientFactory = clientFactory;
        }

        public async Task<T> CreateAsync<T>(UserDTO dto)
        {
            return await _baseServices.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = dto,
                Url = API_URL + $"/api/{SD.CurrentAPIVersion}/User"
            });
        }

        public async Task<T> DeleteAsync<T>(string id)
        {
            return await _baseServices.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.DELETE,
                Url = API_URL + $"/api/{SD.CurrentAPIVersion}/User/" + id
            });
        }

        public async Task<T> GetAllAsync<T>()
        {
            return await _baseServices.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = API_URL + $"/api/{SD.CurrentAPIVersion}/User"
            });
        }

        public async Task<T> GetAsync<T>(string id)
        {
            return await _baseServices.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = API_URL + $"/api/{SD.CurrentAPIVersion}/User/" + id
            });
        }

        public async Task<T> UpdatePasswordAsync<T>(UpdatePasswordRequestDTO updatePasswordRequestDTO)
        {
            return await _baseServices.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = updatePasswordRequestDTO,
                Url = API_URL + $"/api/{SD.CurrentAPIVersion}/User/password/" + updatePasswordRequestDTO.Id
            });
        }

        public async Task<T> UpdateAsync<T>(UserDTO dto)
        {
            return await _baseServices.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = dto,
                Url = API_URL + $"/api/{SD.CurrentAPIVersion}/User/" + dto.Id,
                ContentType = SD.ContentType.MultipartFormData
            });
        }

        public async Task<T> UploadImgAsync<T>(UserDTO dto)
        {
            return await _baseServices.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = dto,
                Url = API_URL + $"/api/{SD.CurrentAPIVersion}/User/UploadImg/" + dto.Id,
                ContentType = SD.ContentType.MultipartFormData
            });
        }
    }
}
