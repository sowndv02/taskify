using System.ComponentModel.DataAnnotations;

namespace taskify_api.Models.DTO
{
    public class TagDTO
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public int ColorId {  get; set; }
        public string? Description { get; set; }
        public ColorDTO? Color { get; set; }
        public UserDTO? User { get; set; }
        public string UserId { get; set; }
        public bool IsDefault { get; set; }
        public List<ProjectTagDTO>? ProjectTags { get; set; } = new List<ProjectTagDTO>();
    }
}
