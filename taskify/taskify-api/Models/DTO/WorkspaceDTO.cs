using System.ComponentModel.DataAnnotations;

namespace taskify_api.Models.DTO
{
    public class WorkspaceDTO
    {
        public int Id { get; set; }
        public UserDTO? Owner { get; set; }
        public string OwnerId { get; set; }
        [Required]
        public string Title { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public virtual List<WorkspaceUserDTO>? WorkspaceUsers { get; set; } = new List<WorkspaceUserDTO>();
        public virtual List<TodoDTO>? Todos { get; set; } = new List<TodoDTO>();
        public virtual List<ActivityLogDTO>? ActivityLogs { get; set; } = new List<ActivityLogDTO>();
    }
}
