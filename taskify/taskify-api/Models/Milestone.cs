using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace taskify_api.Models
{
    public class Milestone
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public DateTime StartAt { get; set; }
        [Required]
        public DateTime EndAt { get; set; }
        public int ProjectId {  get; set; }
        [ForeignKey(nameof(ProjectId))]
        public Project? Project { get; set; }
        public string UserId {  get; set; }
        [ForeignKey(nameof(UserId))]
        public User? User {  get; set; }

        [Range(0, 100)]
        public int Progress { get; set; }
        public bool Status {  get; set; }
        public string Description {  get; set; }
        public DateTime CreatedDate {  get; set; }
        public DateTime? UpdatedDate { get; set;}
    }
}
