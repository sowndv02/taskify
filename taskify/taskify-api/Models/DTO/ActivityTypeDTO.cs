using System.ComponentModel.DataAnnotations;

namespace taskify_api.Models.DTO
{
    public class ActivityTypeDTO
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
