﻿namespace taskify_font_end.Models.DTO
{
    public class UserUpdateDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string ImageUrl { get; set; }
        public string ImageLocalPathUrl { get; set; }
        public IFormFile? Image { get; set; }
    }
}
