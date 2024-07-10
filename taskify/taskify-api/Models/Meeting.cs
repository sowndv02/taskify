using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace taskify_api.Models
{
    public class Meeting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string? RequestId { get; set; }
        public string? MeetingUrl {  get; set; }
        public string OwnerId { get; set; }
        [ForeignKey(nameof(OwnerId))]
        public User? User { get; set; }
        public int WorkspaceId {  get; set; }
        [ForeignKey(nameof(WorkspaceId))]
        public Workspace? Workspace { get; set;}
        public virtual List<MeetingUser> MeetingUsers { get; set; } = new List<MeetingUser>(); 
    }
}
