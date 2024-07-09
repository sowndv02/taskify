using System.ComponentModel.DataAnnotations;

namespace taskify_font_end.Models.DTO
{
    public class UserCreateDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageLocalPathUrl { get; set; }
        public IFormFile? Image { get; set; }
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and ConfirmPassword do not match.")]
        public string ConfirmPassword { get; set; }
        public RoleDTO? Role { get; set; }
        public string? RoleId { get; set; }
        public bool IsLockedOut { get; set; }
    }
}
