using AutoMapper;
using Microsoft.AspNetCore.Identity;
using taskify_font_end.Models.DTO;

namespace taskify_font_end
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<UserDTO, UserUpdateDTO>().ReverseMap();
        }
    }
}
