﻿using Microsoft.AspNetCore.Identity;

namespace taskify_api.Repository.IRepository
{
    public interface IRoleRepository
    {
        Task CreateAsync(IdentityRole role);
        
        Task<List<IdentityRole>> GetAllAsync();
        Task<IdentityRole> GetByNameAsync(string name);
    }
}
