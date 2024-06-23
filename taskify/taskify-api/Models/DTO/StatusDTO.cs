namespace taskify_api.Models.DTO
{
    public class StatusDTO
    {
        public int Id { get; set; }
        public ColorDTO Color { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public UserDTO User { get; set; }

    }
}
