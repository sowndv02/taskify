using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace taskify_api.Models
{
    public class Color
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string ColorCode { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User Owner { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }

    }
}
