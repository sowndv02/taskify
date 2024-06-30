using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
using taskify_font_end.Service.IService;
using taskify_utility;

namespace taskify_font_end.Service
{
    public class TaskUserService : ITaskUserService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string API_URL;
        private readonly IBaseServices _baseServices;

        public TaskUserService(IHttpClientFactory clientFactory, IConfiguration configuration, IBaseServices baseServices)
        {
            _baseServices = baseServices;
            API_URL = configuration.GetValue<string>("ServiceUrls:TaskifyAPI");
            _clientFactory = clientFactory;
        }

        public async Task<T> CreateAsync<T>(TaskUserDTO dto)
        {
            return await _baseServices.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = dto,
                Url = API_URL + $"/api/{SD.CurrentAPIVersion}/taskUser"
            });
        }

        public async Task<T> DeleteAsync<T>(int id)
        {
            return await _baseServices.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.DELETE,
                Url = API_URL + $"/api/{SD.CurrentAPIVersion}/taskUser/" + id
            });
        }

        public async Task<T> GetAllAsync<T>()
        {
            return await _baseServices.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = API_URL + $"/api/{SD.CurrentAPIVersion}/taskUser"
            });
        }

        public async Task<T> GetAsync<T>(int id)
        {
            return await _baseServices.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = API_URL + $"/api/{SD.CurrentAPIVersion}/taskUser/" + id
            });
        }

        public async Task<T> GetByUserIdAsync<T>(string userId)
        {
            return await _baseServices.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = API_URL + $"/api/{SD.CurrentAPIVersion}/taskUser/" + userId
            });
        }

        public async Task<T> UpdateAsync<T>(TaskUserDTO dto)
        {
            return await _baseServices.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = dto,
                Url = API_URL + $"/api/{SD.CurrentAPIVersion}/taskUser/" + dto.Id
            });
        }
        public async Task<T> GetByTaskIdAsync<T>(int id)
        {
            return await _baseServices.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = API_URL + $"/api/{SD.CurrentAPIVersion}/taskUser/" + id
            });
        }
    }
}
