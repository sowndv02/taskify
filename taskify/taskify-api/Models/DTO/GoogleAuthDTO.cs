namespace taskify_api.Models.DTO
{
    public class GoogleAuthDTO
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Address { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageLocalPathUrl { get; set; }
        public IFormFile? Image { get; set; }
        public string Password { get; set; }
        public RoleDTO? Role { get; set; }
        public string? RoleId { get; set; }
    }
}
