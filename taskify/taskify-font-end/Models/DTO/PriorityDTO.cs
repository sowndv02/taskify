
namespace taskify_font_end.Models.DTO
{
    public class PriorityDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ColorId { get; set; }
        public ColorDTO? Color { get; set; }
        public bool IsDefault { get; set; }
        public string UserId { get; set; }
        public UserDTO? User { get; set; }
    }
}
