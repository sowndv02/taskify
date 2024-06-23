using taskify_font_end.Service.IService;

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

        //public async Task InvokeAsync(HttpContext context)
        //{
        //    var token = context.Request.Cookies["AuthToken"];
        //    var refreshToken = context.Request.Cookies["RefreshToken"];

        //    if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(refreshToken))
        //    {
        //        var principal = _tokenProvider.GetPrincipalFromExpiredToken(token);
        //        if (principal != null)
        //        {
        //            var newToken = await _tokenProvider.RefreshTokenAsync(principal, refreshToken);
        //            if (newToken != null)
        //            {
        //                context.Response.Cookies.Append("AuthToken", newToken, new CookieOptions
        //                {
        //                    HttpOnly = true,
        //                    Expires = DateTime.UtcNow.AddMinutes(30)
        //                });
        //            }
        //        }
        //    }

        //    await _next(context);
        //}
    }

    public static class TokenValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenValidationMiddleware>();
        }
    }

}
