using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public virtual List<Milestone> Milestones { get; set; } = new List<Milestone>();
        public virtual List<ProjectMedia> ProjectMedias { get; set; } = new List<ProjectMedia>();
        public virtual List<TaskMedia> TaskMedias { get; set; } = new List<TaskMedia>();
        [NotMapped]
        public IdentityRole? Role { get; set; }
        [NotMapped]
        public string? RoleId { get; set; }
        [NotMapped]
        public bool IsLockedOut { get; set; }
    }
}
