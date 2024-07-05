using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace taskify_api.Models
{
    public class Project
    {
        public Project()
        {
            ProjectTags = new HashSet<ProjectTag>();
            ProjectUsers = new HashSet<ProjectUser>();
            Tasks = new HashSet<TaskModel>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int WorkspaceId { get; set; }
        [ForeignKey(nameof(WorkspaceId))]
        public Workspace? Workspace { get; set; }
        public int StatusId { get; set; }
        [ForeignKey(nameof(StatusId))]
        public Status? Status { get; set; } = null!;
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public DateTime? ActualEndAt { get; set; }
        public string OwnerId { get; set; }
        [ForeignKey(nameof(OwnerId))]
        public User? Owner { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        public ICollection<ProjectUser> ProjectUsers { get; set; }
        public ICollection<TaskModel> Tasks { get; set; }
        public ICollection<ProjectTag> ProjectTags { get; set; }

    }
}
