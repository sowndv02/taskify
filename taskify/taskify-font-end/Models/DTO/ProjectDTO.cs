using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using taskify_font_end.Validation;

namespace taskify_font_end.Models.DTO
{
    public class ProjectDTO
    {
        public int Id { get; set; }
        public int WorkspaceId { get; set; }
        public WorkspaceDTO? Workspace { get; set; }
        public int StatusId { get; set; }
        public StatusDTO? Status { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Notes { get; set; }
        [DateRange("EndAt", ErrorMessage = "StartAt must be earlier than EndAt")]
        public DateTime StartAt { get; set; } = DateTime.Now;
        [Required]
        public DateTime EndAt { get; set; } = DateTime.Now;
        public DateTime? ActualEndAt { get; set; }
        public string OwnerId { get; set; }
        public UserDTO? Owner { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        public List<ProjectUserDTO>? ProjectUsers { get; set; }
        public List<string>? ProjectUserIds { get; set; } = new List<string>();
        public List<int>? ProjectTagIds { get; set; } = new List<int>();
        public List<TaskDTO>? TaskModels { get; set; } = new List<TaskDTO>();
        public List<ProjectTagDTO>? ProjectTags { get; set; } = new List<ProjectTagDTO>();
    }
}
