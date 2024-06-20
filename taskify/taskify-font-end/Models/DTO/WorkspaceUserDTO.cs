namespace taskify_font_end.Models.DTO
{
    public class WorkspaceUserDTO
    {
        public int Id { get; set; }
        public UserDTO User { get; set; }
        public WorkspaceDTO Workspace { get; set; }
    }
}
