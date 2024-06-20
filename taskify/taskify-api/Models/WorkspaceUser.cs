using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace taskify_api.Models
{
    public class WorkspaceUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public int WorkspaceId { get; set; }
        [ForeignKey(nameof(WorkspaceId))]
        public Workspace Workspace { get; set; }
    }
}
