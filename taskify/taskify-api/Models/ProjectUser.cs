using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace taskify_api.Models
{
    public class ProjectUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
        [ForeignKey(nameof(ProjectId))]
        public Project? Project { get; set; }
    }
}
