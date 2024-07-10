using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace taskify_api.Models
{
    public class Workspace
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string OwnerId {  get; set; }
        [ForeignKey(nameof(OwnerId))]
        public virtual User? Owner {  get; set; }
        [Required]
        public string Title {  get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public virtual List<Todo> Todos { get; set; } = new List<Todo>();
        public virtual List<Note> Notes { get; set; } = new List<Note>();
        public virtual List<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
        public virtual List<WorkspaceUser> WorkspaceUsers { get; set; } = new List<WorkspaceUser>();
        public virtual List<Project> Projects { get; set; } = new List<Project>();
        public virtual List<Meeting> Meetings { get; set; } = new List<Meeting>();
    }
}
