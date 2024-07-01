using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace taskify_api.Models
{
    public class ProjectMedia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string FileName {  get; set; }
        public double FileSize {  get; set; }
        public DateTime CreatedDate {  get; set; }
        public DateTime? UpdatedDate {  get; set; }
        public string? MediaUrl { get; set; }
        public int ProjectId { get; set; }
        [ForeignKey(nameof(ProjectId))]
        public Project? Project { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof (UserId))]
        public User? User {  get; set; }
        public string? MediaLocalPathUrl { get; set; }
    }
}
