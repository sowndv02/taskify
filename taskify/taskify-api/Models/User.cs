using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace taskify_api.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string FirstName {  get; set; }
        [Required]
        public string LastName { get; set; }
        public string? Address {  get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageLocalPathUrl { get; set; }
        public virtual List<Color> Colors { get; set; } = new List<Color>();
    }
}
