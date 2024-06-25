using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace taskify_api.Models
{
    public class ProjectTag
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 
        public int ProjectId {  get; set; }
        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }
        public int TagId {  get; set; }
        [ForeignKey(nameof(TagId))]
        public Tag Tag { get; set; }
    }
}
