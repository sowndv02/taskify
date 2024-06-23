﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
using taskify_font_end.Service.IService;
using taskify_utility;

namespace taskify_font_end.Service
{
    public class BaseServices : IBaseServices
    {
        public APIResponse responseModel { get; set; }
        public IHttpClientFactory httpClientFactory { get; set; }
        private readonly ITokenProvider _tokenProvider;
        private readonly IApiMessageRequestBuilder _apiMessageRequestBuilder;
        private readonly string API_URL;
        private IHttpContextAccessor _contextAccessor;
        public BaseServices(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider, IConfiguration configuration
            , IHttpContextAccessor contextAccessor, IApiMessageRequestBuilder apiMessageRequestBuilder)
        {
            API_URL = configuration.GetValue<string>("ServiceUrls:TaskifyAPI");
            responseModel = new();
            this.httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
            _contextAccessor = contextAccessor;
            _apiMessageRequestBuilder = apiMessageRequestBuilder;
        }
        public async Task<T> SendAsync<T>(APIRequest apiRequest, bool withBearer = true)
        {
            try
            {
                var client = httpClientFactory.CreateClient("TaskifyAPI");

                var messageFactory = () =>
                {
                    return _apiMessageRequestBuilder.Build(apiRequest);
                };


                HttpResponseMessage httpResponseMessage = null;
                httpResponseMessage = await SendWithRefreshTokenAsync(client, messageFactory, withBearer);
                APIResponse FinalApiResponse = new()
                {
                    ErrorMessages = new List<string>(),
                    IsSuccess = false
                };
                if (httpResponseMessage != null)
                {
                    var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        FinalApiResponse = JsonConvert.DeserializeObject<APIResponse>(responseContent);
                    }
                }
                else
                {
                    FinalApiResponse = new()
                    {
                        ErrorMessages = new List<string>(),
                        IsSuccess = false
                    };
                }

                try
                {
                    switch (httpResponseMessage.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                            FinalApiResponse.ErrorMessages.Add("Not Found");
                            break;
                        case HttpStatusCode.BadRequest:
                            FinalApiResponse.ErrorMessages.Add("Data input invalid");
                            break;
                        case HttpStatusCode.Forbidden:
                            FinalApiResponse.ErrorMessages.Add("Access Denied");
                            break;
                        case HttpStatusCode.Unauthorized:
                            FinalApiResponse.ErrorMessages.Add("Unauthorized");
                            break;
                        case HttpStatusCode.InternalServerError:
                            FinalApiResponse.ErrorMessages.Add("Internal Server Error");
                            break;
                        default:
                            var apiContent = await httpResponseMessage.Content.ReadAsStringAsync();
                            FinalApiResponse.IsSuccess = true;
                            FinalApiResponse = JsonConvert.DeserializeObject<APIResponse>(apiContent);
                            break;
                    }
                }
                catch (Exception e)
                {
                    FinalApiResponse.ErrorMessages = new List<string>() { "Error Encountered", e.Message.ToString() };
                }
                var res = JsonConvert.SerializeObject(FinalApiResponse);
                var returnObj = JsonConvert.DeserializeObject<T>(res);
                return returnObj;
            }
            catch (AuthenticationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var dto = new APIResponse
                {
                    ErrorMessages = new List<string> { Convert.ToString(ex.Message) },
                    IsSuccess = false
                };
                var res = JsonConvert.SerializeObject(dto);
                var APIResponse = JsonConvert.DeserializeObject<T>(res);
                return APIResponse;
            }
        }

        public async Task<HttpResponseMessage> SendWithRefreshTokenAsync(HttpClient httpClient,
            Func<HttpRequestMessage> htppRequestMessageFactory, bool withBearer = true)
        {
            if (!withBearer)
            {
                return await httpClient.SendAsync(htppRequestMessageFactory());
            }
            else
            {
                TokenDTO tokenDTO = _tokenProvider.GetToken();
                if (tokenDTO != null && string.IsNullOrEmpty(tokenDTO.AccessToken))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenDTO.AccessToken);
                }
                try
                {
                    var response = await httpClient.SendAsync(htppRequestMessageFactory());
                    if (response.IsSuccessStatusCode)
                        return response;

                    // If this fails then we can pass refresh token
                    if (!response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        // Generate new token from refresh token/sign in with that new token and then retry 
                        await InvokeRefreshTokenEndpoint(httpClient, tokenDTO.AccessToken, tokenDTO.RefreshToken);
                        response = await httpClient.SendAsync(htppRequestMessageFactory());
                        return response;
                    }
                    return response;
                }
                catch (AuthenticationException)
                {
                    throw;
                }
                catch (HttpRequestException httpRequestException)
                {
                    if (httpRequestException.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        // refresh token and retry the request
                        await InvokeRefreshTokenEndpoint(httpClient, tokenDTO.AccessToken, tokenDTO.RefreshToken);
                        return await httpClient.SendAsync(htppRequestMessageFactory());
                        throw;
                    }
                }
                return null;
            }
        }


        private async Task InvokeRefreshTokenEndpoint(HttpClient httpClient, string existingAccessToken, string existingRefreshToken)
        {
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");
            message.RequestUri = new Uri($"{API_URL}/api/{SD.CurrentAPIVersion}/UserAuth/refresh");
            message.Method = HttpMethod.Post;
            message.Content = new StringContent(JsonConvert.SerializeObject(new TokenDTO()
            {
                AccessToken = existingAccessToken,
                RefreshToken = existingRefreshToken
            }), Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(message);
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<APIResponse>(content);

            if (apiResponse?.IsSuccess != true)
            {
                await _contextAccessor.HttpContext.SignOutAsync();
                _tokenProvider.ClearToken();
                throw new AuthenticationException();
            }
            else
            {
                var tokenDataStr = JsonConvert.SerializeObject(apiResponse.Result);
                var tokenDto = JsonConvert.DeserializeObject<TokenDTO>(tokenDataStr);

                if (tokenDto != null && !string.IsNullOrEmpty(tokenDto.AccessToken))
                {

                    await SignWithNewTokens(tokenDto);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenDto.AccessToken);
                }
            }
        }

        private async Task SignWithNewTokens(TokenDTO tokenDTO)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(tokenDTO.AccessToken);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == "unique_name").Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));
            var principal = new ClaimsPrincipal(identity);
            await _contextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            _tokenProvider.SetToken(tokenDTO);
        }
    }
}
