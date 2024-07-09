using Microsoft.AspNetCore.Authentication.Cookies;
using taskify_font_end;
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
builder.Services.AddHttpClient<IProjectService, ProjectService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddHttpClient<IActivityService, ActivityService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddHttpClient<IColorService, ColorService>();
builder.Services.AddScoped<IColorService, ColorService>();
builder.Services.AddHttpClient<IStatusService, StatusService>();
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddHttpClient<IActivityTypeService, ActivityTypeService>();
builder.Services.AddScoped<IActivityTypeService, ActivityTypeService>();
builder.Services.AddHttpClient<IActivityLogService, ActivityLogService>();
builder.Services.AddScoped<IActivityLogService, ActivityLogService>();
builder.Services.AddHttpClient<ITagService, TagService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddHttpClient<ITodoService, TodoService>();
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddHttpClient<INoteService, NoteService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddHttpClient<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddHttpClient<ITaskUserService, TaskUserService>();
builder.Services.AddScoped<ITaskUserService, TaskUserService>();
builder.Services.AddHttpClient<IPriorityService, PriorityService>();
builder.Services.AddScoped<IPriorityService, PriorityService>();
builder.Services.AddHttpClient<IProjectUserService, ProjectUserService>();
builder.Services.AddScoped<IProjectUserService, ProjectUserService>();
builder.Services.AddHttpClient<IProjectTagService, ProjectTagService>();
builder.Services.AddScoped<IProjectTagService, ProjectTagService>();
builder.Services.AddHttpClient<IMilestoneService, MilestoneService>();
builder.Services.AddScoped<IMilestoneService, MilestoneService>();
builder.Services.AddHttpClient<IProjectMediaService, ProjectMediaService>();
builder.Services.AddScoped<IProjectMediaService, ProjectMediaService>();
builder.Services.AddHttpClient<ITaskMediaService, TaskMediaService>();
builder.Services.AddScoped<ITaskMediaService, TaskMediaService>();
builder.Services.AddHttpClient<IRoleService, RoleService>();
builder.Services.AddScoped<IRoleService, RoleService>();



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