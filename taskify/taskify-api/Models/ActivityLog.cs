using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static taskify_utility.SD;

namespace taskify_api.Models
{
    public class ActivityLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public int ActivityTypeId {  get; set; }
        [ForeignKey(nameof(ActivityTypeId))]
        public virtual ActivityType ActivityType { get; set; }
        public int ActivityId { get; set; }
        public int WorkspaceId { get; set; }
        [ForeignKey(nameof(WorkspaceId))]
        public virtual Workspace Workspace { get; set; }
        [ForeignKey(nameof(ActivityId))]
        public virtual Activity Activity { get; set; }
        public string TypeTitle { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

    }
}
