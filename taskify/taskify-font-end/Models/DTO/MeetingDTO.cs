using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace taskify_font_end.Models.DTO
{
    public class MeetingDTO
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string? RequestId { get; set; }
        public string? MeetingUrl { get; set; }
        public string OwnerId { get; set; }
        public UserDTO? User { get; set; }
        public int WorkspaceId {  get; set; }
        public WorkspaceDTO? Workspace { get; set; }
        public virtual List<MeetingUserDTO> MeetingUsers { get; set; } = new List<MeetingUserDTO>();
    }
}
