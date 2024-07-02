using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using taskify_font_end.Validation;

namespace taskify_font_end.Models.DTO
{
    public class MilestoneDTO
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [DateRange("EndAt", ErrorMessage = "StartAt must be earlier than EndAt")]
        public DateTime StartAt { get; set; }
        [Required]
        public DateTime EndAt { get; set; }
        public int ProjectId { get; set; }
        public ProjectDTO? Project { get; set; }
        public string UserId { get; set; }
        public UserDTO? User { get; set; }

        [Range(0, 100)]
        public int Progress { get; set; }
        public bool Status { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
