using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
using taskify_font_end.Service.IService;
using taskify_utility;

namespace taskify_font_end.Middlewares
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAuthService _authService;
        private readonly HttpClient _httpClient;
        private string API_URL;

        public TokenValidationMiddleware(RequestDelegate next, HttpClient httpClient, IConfiguration configuration)
        {
            API_URL = configuration.GetValue<string>("ServiceUrls:TaskifyAPI");
            _next = next;
            _httpClient = httpClient;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/Auth/Login"))
            {
                await _next(context);
                return;
            }
            var authCookie = context.Request.Cookies[CookieAuthenticationDefaults.CookiePrefix + CookieAuthenticationDefaults.AuthenticationScheme];

            if (string.IsNullOrEmpty(authCookie))
            {
                var token = context.Request.Headers[SD.AccessToken].ToString();
                var refreshToken = context.Request.Headers[SD.AccessToken].ToString();

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refreshToken))
                {
                    RedirectToLogin(context);
                    return;
                }

                // Call the TokenProvider to re-authenticate
                var response = await ReAuthenticateAsync(new TokenDTO { AccessToken = token, RefreshToken = refreshToken });
                if (response == null || !response.IsSuccess || response.ErrorMessages.Count != 0)
                {
                    RedirectToLogin(context);
                    return;
                }

                TokenDTO tokenDTO = JsonConvert.DeserializeObject<TokenDTO>(Convert.ToString(response.Result));

                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(tokenDTO.AccessToken);

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == "unique_name").Value));
                identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, jwt.Claims.FirstOrDefault(u => u.Type == "sub").Value));
                identity.AddClaim(new Claim(ClaimTypes.GivenName, jwt.Claims.FirstOrDefault(u => u.Type == "given_name").Value));
                identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, "user"));
                var principal = new ClaimsPrincipal(identity);
                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                {
                    IsPersistent = true, // Persistent cookie
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Cookie expiration set to 30 minutes
                });

                context.Response.Cookies.Delete(SD.AccessToken);
                context.Response.Cookies.Delete(SD.RefreshToken);

                var cookieOptions = new CookieOptions { Expires = DateTime.UtcNow.AddDays(60) };
                context.Response.Cookies.Append(SD.AccessToken, tokenDTO.AccessToken, cookieOptions);
                context.Response.Cookies.Append(SD.RefreshToken, tokenDTO.RefreshToken, cookieOptions);
            }
            await _next(context);
        }
        private void RedirectToLogin(HttpContext context)
        {
            context.Response.Redirect("/Auth/Login");
        }

        private async Task<APIResponse> ReAuthenticateAsync(TokenDTO tokenDTO)
        {
            var url = API_URL + $"/api/{SD.CurrentAPIVersion}/UserAuth/refresh";
            var content = new StringContent(JsonConvert.SerializeObject(tokenDTO), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            var apiResponse = new APIResponse();
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                apiResponse = JsonConvert.DeserializeObject<APIResponse>(responseBody);
            }
            else
            {
                apiResponse.IsSuccess = false;
                apiResponse.ErrorMessages.Add("Re-authentication failed");
            }

            return apiResponse;
        }
        private class AuthResponse
        {
            public bool IsAuthenticated { get; set; }
        }
    }

    public static class TokenValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenValidationMiddleware>();
        }
    }

}
