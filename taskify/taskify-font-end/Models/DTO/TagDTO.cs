using System.ComponentModel.DataAnnotations;

namespace taskify_font_end.Models.DTO
{
    public class TagDTO
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public ColorDTO? Color { get; set; }
        public int ColorId { get; set; }
        public bool IsDefault { get; set; }
        public string UserId { get; set; }
        public List<ProjectTagDTO>? ProjectTags { get; set; } = new List<ProjectTagDTO>();
    }
}
