﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using taskify_api.Models;
using taskify_api.Models.DTO;

namespace taskify_api
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<RoleDTO, IdentityRole>().ReverseMap();
            CreateMap<UserDTO, User>().ReverseMap();
            CreateMap<ColorDTO, Color>().ReverseMap();
        }
    }
}
