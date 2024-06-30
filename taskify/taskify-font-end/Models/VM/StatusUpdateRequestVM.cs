namespace taskify_font_end.Models.VM
{
    public class StatusUpdateRequestVM
    {
        public int Id { get; set; }
        public bool Status { get; set; }
        public string UserId { get; set; }
    }
}
