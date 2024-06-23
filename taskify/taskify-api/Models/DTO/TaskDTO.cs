using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace taskify_api.Models.DTO
{
    public class TaskDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int StatusId { get; set; }
        public Status Status { get; set; }
        public int PriorityId { get; set; }
        public Priority Priority { get; set; }
        public string OwnerId { get; set; }
        public User Owner { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
    }
}
