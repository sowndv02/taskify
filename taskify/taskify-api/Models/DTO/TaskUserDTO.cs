using System.ComponentModel.DataAnnotations.Schema;

namespace taskify_api.Models.DTO
{
    public class TaskUserDTO
    {
        public int Id { get; set; }

        public int TaskId { get; set; }
        public string UserId { get; set; }
        public UserDTO? User { get; set; }
        public TaskDTO? Task { get; set; }
    }
}
