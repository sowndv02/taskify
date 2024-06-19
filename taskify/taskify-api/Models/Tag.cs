using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace taskify_api.Models
{
    public class Tag
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Title {  get; set; }
        public string? Description { get; set; }
        public int ColorId {  get; set; }
        [ForeignKey(nameof(ColorId))]
        public virtual Color color {  get; set; }
    }
}
