using System.ComponentModel.DataAnnotations;

namespace taskify_api.Models.DTO
{
    public class UserDTO
    {
        public string UserId {  get; set; }
        public string Email {  get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Address { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageLocalPathUrl { get; set; }
    }
}
