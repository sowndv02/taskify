using System.ComponentModel.DataAnnotations.Schema;

namespace taskify_font_end.Models.DTO
{
    public class MeetingUserDTO
    {
        public int Id { get; set; } 
        public string UserId { get; set; }
        public int MeetingId { get; set; }
        public UserDTO? User { get; set; }
        public MeetingDTO? Meeting { get; set; }
    }
}
