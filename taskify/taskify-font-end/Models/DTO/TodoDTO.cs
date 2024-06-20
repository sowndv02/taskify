using System.ComponentModel.DataAnnotations;

namespace taskify_font_end.Models.DTO
{
    public class TodoDTO
    {
        public int Id { get; set; }
        public WorkspaceDTO Workspace { get; set; }
        public bool Status { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public UserDTO User { get; set; }
    }
}
