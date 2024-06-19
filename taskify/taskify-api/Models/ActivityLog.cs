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
        public User User { get; set; }

        public ActivityType ActivityType { get; set; }

        public string Type { get; set; }
        public string TypeTitle { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
