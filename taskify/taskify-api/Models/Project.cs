using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace taskify_api.Models
{
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int WorkspaceId { get; set; }
        [ForeignKey(nameof(WorkspaceId))]
        public Workspace Workspace { get; set; }
        public int StatusId { get; set; }
        [ForeignKey(nameof(StatusId))]
        public Status Status { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public DateTime? ActualEndAt { get; set; }
        public string OwnerId { get; set; }
        [ForeignKey(nameof(OwnerId))]
        public User Owner { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        public List<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();
        public List<TaskModel> TaskModels { get; set; } = new List<TaskModel>();
    }
}
