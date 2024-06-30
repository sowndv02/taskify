using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace taskify_api.Models
{
    public class Priority
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Title {  get; set; }
        public int ColorId {  get; set; }
        [ForeignKey(nameof(ColorId))]
        public Color? Color { get; set; }
        public bool IsDefault { get; set; }
        public string UserId {  get; set; }
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
    }
}
