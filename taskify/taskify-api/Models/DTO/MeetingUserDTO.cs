namespace taskify_api.Models.DTO
{
    public class MeetingUserDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int MeetingId { get; set; }
        public UserDTO? User { get; set; }
    }
}
