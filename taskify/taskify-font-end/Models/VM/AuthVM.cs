using taskify_font_end.Models.DTO;

namespace taskify_font_end.Models.VM
{
    public class AuthVM
    {
        public RegisterationRequestDTO? RegisterationRequest { get; set; }
        public LoginRequestDTO? LoginRequest { get; set; }
    }
}
