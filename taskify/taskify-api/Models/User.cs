using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace taskify_api.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string? Address { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageLocalPathUrl { get; set; }
        public virtual List<Color>? Colors { get; set; } = new List<Color>();
        public virtual List<Project>? Projects { get; set; } = new List<Project>();
        public virtual List<ProjectUser>? ProjectUsers { get; set; } = new List<ProjectUser>();
        public virtual List<TaskUser>? TaskUsers { get; set; } = new List<TaskUser>();
        public virtual List<Priority>? Priorities { get; set; } = new List<Priority>();

    }
}
