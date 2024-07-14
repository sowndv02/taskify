using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
using taskify_font_end.Service.IService;
using taskify_utility;

namespace taskify_font_end.Service
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHttpClientFactory _clientFactory;
        private string API_URL;
        private readonly IBaseServices _baseServices;
        public TokenProvider(IHttpContextAccessor contextAccessor, IBaseServices baseServices, IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _baseServices = baseServices;
            API_URL = configuration.GetValue<string>("ServiceUrls:TaskifyAPI");
            _clientFactory = clientFactory;
            _contextAccessor = contextAccessor;
        }

        public void ClearToken()
        {
            _contextAccessor.HttpContext?.Response.Cookies.Delete(SD.AccessToken);
            _contextAccessor.HttpContext?.Response.Cookies.Delete(SD.RefreshToken);
        }

        public TokenDTO GetToken()
        {
            try
            {
                bool hasAccessToken = _contextAccessor.HttpContext.Request.Cookies.TryGetValue(SD.AccessToken, out string accessToken);
                bool hasRefreshToken = _contextAccessor.HttpContext.Request.Cookies.TryGetValue(SD.RefreshToken, out string refreshToken);

                TokenDTO tokenDTO = new()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };
                return hasAccessToken ? tokenDTO : null;

            }
            catch (Exception ex)
            {
                return null;
            }

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
        public async Task<T> ReAuthenticateAsync<T>(TokenDTO tokenDTO)
        {
            return await _baseServices.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = tokenDTO,
                Url = API_URL + $"/api/{SD.CurrentAPIVersion}/UserAuth/refresh"
            }, withBearer: false);

        }


        public void SetToken(TokenDTO tokenDTO)
        {
            var cookieOptions = new CookieOptions { Expires = DateTime.UtcNow.AddDays(60) };
            _contextAccessor.HttpContext?.Response.Cookies.Append(SD.AccessToken, tokenDTO.AccessToken, cookieOptions);
            _contextAccessor.HttpContext?.Response.Cookies.Append(SD.RefreshToken, tokenDTO.RefreshToken, cookieOptions);
        }
    }
}
