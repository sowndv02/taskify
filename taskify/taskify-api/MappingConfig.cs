using AutoMapper;
using Microsoft.AspNetCore.Identity;
using taskify_api.Models;
using taskify_api.Models.DTO;

namespace taskify_api
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<RoleDTO, IdentityRole>().ReverseMap();
            CreateMap<UserDTO, User>().ReverseMap();
            CreateMap<ColorDTO, Color>().ReverseMap();
            CreateMap<ActivityDTO, Activity>().ReverseMap();
            CreateMap<ActivityLogDTO, ActivityLog>().ReverseMap();
            CreateMap<WorkspaceDTO, Workspace>().ReverseMap();
            CreateMap<ActivityTypeDTO, ActivityType>().ReverseMap();
            CreateMap<StatusDTO, Status>().ReverseMap();
            CreateMap<TagDTO, Tag>().ReverseMap();
            CreateMap<TodoDTO, Todo>().ReverseMap();
            CreateMap<NoteDTO, Note>().ReverseMap();
            CreateMap<WorkspaceUserDTO, WorkspaceUser>().ReverseMap();
            CreateMap<PriorityDTO, Priority>().ReverseMap();
            CreateMap<TaskDTO, TaskModel>().ReverseMap();
            CreateMap<TaskUserDTO, TaskUser>().ReverseMap();
            CreateMap<ProjectDTO, Project>().ReverseMap();
            CreateMap<ProjectUserDTO, ProjectUser>().ReverseMap();
            CreateMap<ProjectTagDTO, ProjectTag>().ReverseMap();
            CreateMap<TaskMediaDTO, TaskMedia>().ReverseMap();
            CreateMap<ProjectMediaDTO, ProjectMedia>().ReverseMap();
            CreateMap<MilestoneDTO, Milestone>().ReverseMap();
        }
    }
}
