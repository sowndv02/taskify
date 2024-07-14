using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using taskify_font_end.Models;
using taskify_font_end.Models.DTO;
using taskify_font_end.Service.IService;
using taskify_utility;

namespace taskify_font_end.Middlewares
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITokenProvider _tokenProvider;

        public TokenValidationMiddleware(RequestDelegate next, ITokenProvider tokenProvider)
        {
            _next = next;
            _tokenProvider = tokenProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var authCookie = context.Request.Cookies["AuthCookie"];

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
                var response = await _tokenProvider.ReAuthenticateAsync<APIResponse>(new TokenDTO { AccessToken = token, RefreshToken = refreshToken});
                if(response == null || !response.IsSuccess || response.ErrorMessages.Count != 0)
                {
                    RedirectToLogin(context);
                    return;
                }

                TokenDTO model = JsonConvert.DeserializeObject<TokenDTO>(Convert.ToString(response.Result));

                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(model.AccessToken);

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
                _tokenProvider.SetToken((TokenDTO)response.Result);
                
            }
            await _next(context);
        }
        private void RedirectToLogin(HttpContext context)
        {
            context.Response.Redirect("/Auth/Login");
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
