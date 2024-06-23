using Microsoft.AspNetCore.Authentication.Cookies;
using taskify_font_end;
using taskify_font_end.Middlewares;
using taskify_font_end.Service;
using taskify_font_end.Service.IService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddAutoMapper(typeof(MappingConfig));

// Common DI
builder.Services.AddScoped<IBaseServices, BaseServices>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpClient<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IApiMessageRequestBuilder, ApiMessageRequestBuilder>();

// Other DI
builder.Services.AddHttpClient<IWorkspaceService, WorkspaceService>();
builder.Services.AddScoped<IWorkspaceService, WorkspaceService>();
builder.Services.AddHttpClient<IUserService, UserService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddHttpClient<IWorkspaceUserService, WorkspaceUserService>();
builder.Services.AddScoped<IWorkspaceUserService, WorkspaceUserService>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.SlidingExpiration = true;
    });

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
//app.UseTokenValidation();




app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=LandingPage}/{id?}");

app.Run();