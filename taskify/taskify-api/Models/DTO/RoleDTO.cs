using System.ComponentModel.DataAnnotations;

namespace taskify_api.Models.DTO
{
    public class RoleDTO
    {
        public string Id {  get; set; } 
        [Required]
        public string Name {  get; set; }
    }
}
