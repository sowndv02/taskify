using System.ComponentModel.DataAnnotations;

namespace taskify_font_end.Models.DTO
{
    public class TagDTO
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public ColorDTO color { get; set; }
    }
}
