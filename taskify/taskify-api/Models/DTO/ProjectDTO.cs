using System.ComponentModel.DataAnnotations.Schema;

namespace taskify_api.Models.DTO
{
    public class ProjectDTO
    {
        public int Id { get; set; }
        public int WorkspaceId { get; set; }
        public WorkspaceDTO Workspace { get; set; }
        public int StatusId { get; set; }
        public StatusDTO Status { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public DateTime? ActualEndAt { get; set; }
        public string OwnerId { get; set; }
        public UserDTO Owner { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        public List<ProjectUserDTO> ProjectUsers { get; set; }
        public List<TaskDTO> TaskModels { get; set; } = new List<TaskDTO>();
    }
}
