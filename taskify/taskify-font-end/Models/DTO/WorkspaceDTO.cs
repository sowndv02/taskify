using System.ComponentModel.DataAnnotations;

namespace taskify_font_end.Models.DTO
{
    public class WorkspaceDTO
    {
        public int Id { get; set; }
        public UserDTO Owner { get; set; }
        [Required]
        public string Title { get; set; }
    }
}
