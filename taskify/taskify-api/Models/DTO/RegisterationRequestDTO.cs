using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace taskify_api.Models.DTO
{
    public class RegisterationRequestDTO
    {
        [Required]
        [EmailAddress]
        public string UserName { get; set; }
        [Required]
        [PasswordPropertyText]
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        public string Role { get; set; } = "client";
    }
}
