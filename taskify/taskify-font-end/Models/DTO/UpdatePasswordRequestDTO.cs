namespace taskify_font_end.Models.DTO
{
    public class UpdatePasswordRequestDTO
    {
        public string Id {  get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
    }
}
