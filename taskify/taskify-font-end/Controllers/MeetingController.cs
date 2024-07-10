using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using taskify_font_end.Service.IService;

namespace taskify_font_end.Controllers
{
    public class MeetingController : BaseController
    {
        private readonly IProjectService _projectService;
        private readonly IWorkspaceService _workspaceService;
        private readonly IWorkspaceUserService _workspaceUserService;
        private readonly IUserService _userService;
        private readonly IStatusService _statusService;
        private readonly ITagService _tagService;
        private readonly IProjectUserService _projectUserService;
        private readonly IProjectTagService _projectTagService;
        private readonly ITaskService _taskService;
        private readonly ITaskUserService _taskUserService;
        private readonly IMilestoneService _milestoneService;
        private readonly IProjectMediaService _projectMediaService;
        private readonly IMapper _mapper;

        public MeetingController(IProjectService projectService, IMapper mapper,
            IWorkspaceService workspaceService, IUserService userService,
            IStatusService statusService, ITagService tagService,
            IProjectUserService projectUserService, IProjectTagService projectTagService,
            ITaskService taskService, ITaskUserService taskUserService,
            IWorkspaceUserService workspaceUserService,
            IMilestoneService milestoneService,
            IProjectMediaService projectMediaService,
            IConfiguration configuration) : base(workspaceService, workspaceUserService)
        {
            _workspaceUserService = workspaceUserService;
            _workspaceService = workspaceService;
            _mapper = mapper;
            _projectService = projectService;
            _userService = userService;
            _statusService = statusService;
            _tagService = tagService;
            _taskService = taskService;
            _projectUserService = projectUserService;
            _projectTagService = projectTagService;
            _taskUserService = taskUserService;
            _milestoneService = milestoneService;
            _projectMediaService = projectMediaService;
        }
    }
}
