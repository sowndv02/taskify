﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using taskify_api.Data;
using taskify_api.Models;
using taskify_api.Repository.IRepository;

namespace taskify_api.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        public RoleRepository(RoleManager<IdentityRole> roleManager, ApplicationDbContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }
        public async Task CreateAsync(IdentityRole objAdd)
        {
            try
            {
                if (_roleManager.RoleExistsAsync(objAdd.Name).GetAwaiter().GetResult())
                {
                    return;
                }
                IdentityRole role = new IdentityRole()
                {
                    Name = objAdd.Name,
                };
                await _roleManager.CreateAsync(role);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<IdentityRole>> GetAllAsync()
        {
            try
            {
                return await _roleManager.Roles.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<IdentityRole> GetRoleByUserId(string userId)
        {
            try
            {
                if (!string.IsNullOrEmpty(userId))
                {
                    var user = await _userManager.FindByIdAsync(userId);

                    var roleIds = await _userManager.GetRolesAsync(user);
                    var role = await _roleManager.FindByNameAsync(roleIds.FirstOrDefault());
                    return role;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<IdentityRole> GetByNameAsync(string name)
        {
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    var role = await _roleManager.FindByNameAsync(name);
                    return role;
                }


                return null;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
