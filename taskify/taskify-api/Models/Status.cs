using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace taskify_api.Models
{
    public class Status
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ColorId { get; set; }
        [ForeignKey(nameof(ColorId))]
        public virtual Color? Color { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User? Owner { get; set; }
        public bool IsDefault { get; set; }
        public List<TaskModel> TaskModels { get; set; } = new List<TaskModel>();


    }
}
