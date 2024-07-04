using System.ComponentModel.DataAnnotations;

namespace taskify_font_end.Models.DTO
{
    public class TaskMediaDTO
    {
        public int Id { get; set; }
        [Required]
        public string FileName { get; set; }
        public double FileSize { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? MediaUrl { get; set; }
        public int TaskId { get; set; }
        public TaskDTO? Task { get; set; }
        public string UserId { get; set; }
        public UserDTO? User { get; set; }
        public string? MediaLocalPathUrl { get; set; }
        public IFormFile? File { get; set; }
    }
}
