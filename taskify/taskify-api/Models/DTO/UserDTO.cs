﻿namespace taskify_api.Models.DTO
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Address { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageLocalPathUrl { get; set; }
        public virtual List<ColorDTO>? Colors { get; set; } = new List<ColorDTO>();
        public virtual List<ProjectDTO>? Projects { get; set; } = new List<ProjectDTO>();
        public virtual List<ProjectUserDTO>? ProjectUsers { get; set; } = new List<ProjectUserDTO>();
        public virtual List<TaskUserDTO>? TaskUsers { get; set; } = new List<TaskUserDTO>();
    }
}
