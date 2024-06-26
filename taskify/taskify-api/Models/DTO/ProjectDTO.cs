using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using taskify_api.Validation;

namespace taskify_api.Models.DTO
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
        public DateTime StartAt { get; set; }
        [Required]
        public DateTime EndAt { get; set; }
        public DateTime? ActualEndAt { get; set; }
        public string OwnerId { get; set; }
        [JsonIgnore]
        public UserDTO? Owner { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        [JsonIgnore]
        public List<ProjectUserDTO>? ProjectUsers { get; set; } = new List<ProjectUserDTO>();
        public List<TaskDTO>? TaskModels { get; set; } = new List<TaskDTO>();
        [JsonIgnore]
        public List<ProjectTagDTO>? ProjectTags { get; set; } = new List<ProjectTagDTO>();
    }
}
