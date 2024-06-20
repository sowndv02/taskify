namespace taskify_api.Models.DTO
{
    public class NoteDTO
    {
        public int Id { get; set; }
        public WorkspaceDTO Workspace { get; set; }
        public ColorDTO Color { get; set; }
        public UserDTO User { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
    }
}
