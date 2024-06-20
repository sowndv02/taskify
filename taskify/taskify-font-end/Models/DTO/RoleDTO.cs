using System.ComponentModel.DataAnnotations;

namespace taskify_font_end.Models.DTO
{
    public class RoleDTO
    {
        public string? Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
