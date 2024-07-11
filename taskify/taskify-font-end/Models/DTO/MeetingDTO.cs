using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace taskify_font_end.Models.DTO
{
    public class MeetingDTO
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Token { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string? RequestId { get; set; }
        public string? MeetingUrl { get; set; }
        public string OwnerId { get; set; }
        public UserDTO? User { get; set; }
        public int WorkspaceId {  get; set; }
        public WorkspaceDTO? Workspace { get; set; }
        public virtual List<MeetingUserDTO> MeetingUsers { get; set; } = new List<MeetingUserDTO>();
        public virtual List<string> MeetingUserIds { get; set; } = new List<string>();
    }
}
