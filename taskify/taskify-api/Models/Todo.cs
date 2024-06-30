using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace taskify_api.Models
{
    public class Todo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int WorkspaceId { get; set; }
        [ForeignKey(nameof(WorkspaceId))]
        public Workspace? Workspace { get; set; }
        public bool Status { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
        public int PriorityId {  get; set; }
        [ForeignKey(nameof(PriorityId))]
        public Priority? Priority { get; set; } 
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}
