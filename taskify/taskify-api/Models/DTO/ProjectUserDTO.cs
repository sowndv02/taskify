using System.ComponentModel.DataAnnotations.Schema;

namespace taskify_api.Models.DTO
{
    public class ProjectUserDTO
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string UserId { get; set; }
        public UserDTO User { get; set; }
        public ProjectDTO Project { get; set; }
    }
}
