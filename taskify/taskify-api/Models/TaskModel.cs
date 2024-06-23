using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace taskify_api.Models
{
    [Table("Task")]
    public class TaskModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Title {  get; set; }
        public int StatusId {  get; set; }
        [ForeignKey(nameof(StatusId))]
        public Status Status { get; set; }
        public int PriorityId {  get; set; }
        [ForeignKey(nameof(PriorityId))]
        public Priority Priority { get; set; }
        public string OwnerId { get; set; }
        [ForeignKey(nameof(OwnerId))]
        public User Owner { get; set; }
        public int ProjectId {  get; set; }
        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate {  get; set; } = DateTime.Now;
        public DateTime? UpdatedDate {  get; set; }
        public virtual List<TaskUser> TaskUsers { get; set; } = new List<TaskUser>();

    }
}
