using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
using taskify_font_end.Service.IService;
using taskify_utility;

namespace taskify_font_end.Service
{
    public class AuthService : IAuthService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string API_URL;
        private readonly IBaseServices _baseServices;
        public AuthService(IHttpClientFactory clientFactory, IConfiguration configuration, IBaseServices baseServices)
        {
            _baseServices = baseServices;
            _clientFactory = clientFactory;
            API_URL = configuration.GetValue<string>("ServiceUrls:TaskifyAPI");

        }


        public async Task<T> LoginAsync<T>(LoginRequestDTO obj)
        {
            return await _baseServices.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = obj,
                Url = API_URL + $"/api/{SD.CurrentAPIVersion}/UserAuth/login"
            }, withBearer: false);
        }

        public async Task<T> RegisterAsync<T>(RegisterationRequestDTO obj)
        {
            return await _baseServices.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = obj,
                Url = API_URL + $"/api/{SD.CurrentAPIVersion}/UserAuth/register"
            }, withBearer: false);
        }

        public async Task<T> AuthenticateWithGoogle<T>(GoogleAuthDTO googleAuthDTO)
        {
            return await _baseServices.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = googleAuthDTO,
                Url = API_URL + $"/api/{SD.CurrentAPIVersion}/UserAuth/google"
            }, withBearer: false);
        }

        public async Task<T> LogoutAsync<T>(TokenDTO obj)
        {
            return await _baseServices.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = obj,
                Url = API_URL + $"/api/{SD.CurrentAPIVersion}/UserAuth/revoke"
            });
        }
    }
}
