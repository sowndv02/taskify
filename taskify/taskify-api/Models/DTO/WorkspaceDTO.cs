using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace taskify_api.Models.DTO
{
    public class WorkspaceDTO
    {
        public int Id { get; set; }
        public User Owner { get; set; }
        [Required]
        public string Title { get; set; }
    }
}
