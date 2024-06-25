using System.ComponentModel.DataAnnotations;

namespace taskify_api.Models.DTO
{
    public class ColorDTO
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string ColorCode { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public UserDTO User { get; set; }

    }
}
