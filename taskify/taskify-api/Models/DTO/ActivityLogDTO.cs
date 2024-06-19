using System.ComponentModel.DataAnnotations.Schema;

namespace taskify_api.Models.DTO
{
    public class ActivityLogDTO
    {
        public int Id { get; set; }
        public User User { get; set; }
        public ActivityTypeDTO ActivityType { get; set; }
        public WorkspaceDTO Workspace {  get; set; } 
        public string Type { get; set; }
        public string TypeTitle { get; set; }
    }
}
