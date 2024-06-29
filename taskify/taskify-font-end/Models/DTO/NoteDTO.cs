namespace taskify_font_end.Models.DTO
{
    public class NoteDTO
    {
        public int Id { get; set; }
        public WorkspaceDTO? Workspace { get; set; }
        public ColorDTO? Color { get; set; }
        public UserDTO? User { get; set; }
        public string Title { get; set; }
        public string UserId { get; set; }
        public int WorkspaceId { get; set; }
        public int ColorId { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
