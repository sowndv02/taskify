namespace taskify_font_end.Models.DTO
{
    public class StatusDTO
    {
        public int Id { get; set; }
        public ColorDTO Color { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public List<TaskDTO> TaskModels { get; set; } = new List<TaskDTO>();
    }
}
