﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using taskify_api.Data;
using taskify_api.Models;
using taskify_api.Models.DTO;
using taskify_api.Repository.IRepository;
using taskify_utility;

namespace taskify_api.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private string secretKey = string.Empty;

        public UserRepository(ApplicationDbContext context, IConfiguration configuration, UserManager<User> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _mapper = mapper;
            _userManager = userManager;
            _context = context;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        public async Task<User> UpdateAsync(User entity)
        {
            try
            {
                _context.Users.Update(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool IsUniqueUser(string username)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserName == username);
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public async Task<TokenDTO> Login(LoginRequestDTO loginRequestDTO, bool checkPassword = true)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName.ToLower() == loginRequestDTO.UserName.ToLower());
            if (user == null)
            {
                return new TokenDTO()
                {
                    AccessToken = "",
                    RefreshToken = ""
                };
            }
            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
            {
                return null;
            }
            bool isValid = false;
            if (checkPassword)
                isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
            else isValid = true;
            if (!isValid)
            {
                return new TokenDTO()
                {
                    AccessToken = ""
                };
            }
            var jwtTokenId = $"JTI{Guid.NewGuid()}";
            var accessToken = await GetAccessToken(user, jwtTokenId);
            var refreshToken = await CreateNewRefreshToken(user.Id, jwtTokenId);

            TokenDTO tokenDTO = new()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
            return tokenDTO;
        }


        public async Task<User> Register(RegisterationRequestDTO registerationRequestDTO)
        {
            User user = new()
            {
                UserName = registerationRequestDTO.UserName,
                Email = registerationRequestDTO.UserName,
                FirstName = registerationRequestDTO.FirstName,
                LastName = registerationRequestDTO.LastName,
                PhoneNumber = registerationRequestDTO.PhoneNumber,
                NormalizedEmail = registerationRequestDTO.UserName,
                PasswordHash = registerationRequestDTO.Password,
                LockoutEnabled = true
            };
            try
            {
                var result = await _userManager.CreateAsync(user, registerationRequestDTO.Password);
                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync(registerationRequestDTO.Role).GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole(registerationRequestDTO.Role));
                    }

                    await _userManager.AddToRoleAsync(user, registerationRequestDTO.Role);
                    return _context.Users.FirstOrDefault(u => u.UserName == registerationRequestDTO.UserName);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

        private async Task<string> GetAccessToken(User user, string jwtTokenId)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, $"{user.FirstName.ToString()} {user.LastName.ToString()}"),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                    new Claim(JwtRegisteredClaimNames.Jti, jwtTokenId),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.GivenName, user.ImageUrl),
                    new Claim(JwtRegisteredClaimNames.Aud, "sonwdv02.com")
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                Issuer = "https://taskify-sondv.com",
                Audience = "https://test-taskify-api.com",
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenStr = tokenHandler.WriteToken(token);
            return tokenStr;
        }

        public async Task<TokenDTO> RefreshAccessToken(TokenDTO tokenDTO)
        {
            // Find an existing refresh token

            var existingRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Refresh_Token == tokenDTO.RefreshToken);
            if (existingRefreshToken == null)
            {
                return new TokenDTO();
            }

            // Compare data from existing refresh token provided and if there is any missmatch then consider it as a fraud
            var isTokenValid = GetAccessTokenData(tokenDTO.AccessToken, existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
            if (!isTokenValid)
            {
                await MarkTokenAsInValid(existingRefreshToken);
                return new TokenDTO();
            }

            // When someone tries to use not valid refresh token, fraud possible
            if (!existingRefreshToken.IsValid)
            {
                MarkAllTokenInChainAsInValid(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
                return new TokenDTO();
            }


            // If jus expired then mark as invalid and return empty
            if (existingRefreshToken.ExpiresAt < DateTime.UtcNow)
            {
                await MarkTokenAsInValid(existingRefreshToken);
                return new TokenDTO();
            }

            // Replace old refresh with a new one with updated expire date
            var newRefreshToken = await CreateNewRefreshToken(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);


            // revoke existing refresh token
            await MarkTokenAsInValid(existingRefreshToken);

            // generate new access token
            var applicationUser = _context.Users.FirstOrDefault(x => x.Id == existingRefreshToken.UserId);
            if (applicationUser == null)
                return new TokenDTO();

            var newAccessToken = await GetAccessToken(applicationUser, existingRefreshToken.JwtTokenId);
            return new TokenDTO()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        private async Task<string> CreateNewRefreshToken(string userId, string tokenId)
        {
            RefreshToken refreshToken = new()
            {
                IsValid = true,
                UserId = userId,
                JwtTokenId = tokenId,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                Refresh_Token = Guid.NewGuid() + "-" + Guid.NewGuid()
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
            return refreshToken.Refresh_Token;
        }

        private bool GetAccessTokenData(string accessToken, string expectedUserId, string expectedTokenId)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(accessToken);

                var jwtTolenId = jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Jti).Value;
                var userId = jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
                return userId == expectedUserId && jwtTolenId == expectedTokenId;

            }
            catch
            {
                return false;
            }
        }


        private void MarkAllTokenInChainAsInValid(string userId, string tokenId)
        {
            var chainRecords = _context.RefreshTokens.Where(x => x.UserId == userId
                && x.JwtTokenId == tokenId);
            foreach (var item in chainRecords)
            {
                item.IsValid = false;
            }
            _context.UpdateRange(chainRecords);
            _context.SaveChanges();
        }

        private async Task MarkTokenAsInValid(RefreshToken refreshToken)
        {
            refreshToken.IsValid = false;
            await _context.SaveChangesAsync();
        }

        public async Task RevokeRefreshToken(TokenDTO tokenDTO)
        {
            var existingRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(_ => _.Refresh_Token == tokenDTO.RefreshToken);
            if (existingRefreshToken == null)
                return;

            // Compare data from existing refresh and access token provided and 
            // if there is any missmatch then we should do nothing with refresh token

            var isTokenValid = GetAccessTokenData(tokenDTO.AccessToken, existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
            if (!isTokenValid)
            {
                return;
            }
            MarkAllTokenInChainAsInValid(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
        }

        public async Task<User> GetAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                user.Role = await GetRoleByUserId(user);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    user.Role = await GetRoleByUserId(user);
                    return user;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> UnlockUser(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.LockoutEnd = null;
                    await _userManager.UpdateAsync(user);
                }
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> LockoutUser(string userId)
        {
            try
            {

                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.LockoutEnd = DateTime.MaxValue;
                    await _userManager.UpdateAsync(user);
                }
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> UpdatePasswordAsync(UpdatePasswordRequestDTO updatePasswordRequestDTO)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserName.ToLower() == updatePasswordRequestDTO.UserName.ToLower());

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            bool isValid = await _userManager.CheckPasswordAsync(user, updatePasswordRequestDTO.Password);

            if (!isValid)
            {
                throw new Exception("Invalid current password.");
            }

            var result = await _userManager.ChangePasswordAsync(user, updatePasswordRequestDTO.Password, updatePasswordRequestDTO.NewPassword);

            if (!result.Succeeded)
            {
                throw new Exception("Password change failed.");
            }

            return user;

        }
        public async Task<User> CreateAsync(User user, string password)
        {
            User obj = new User
            {
                UserName = user.UserName,
                ImageLocalPathUrl = user.ImageLocalPathUrl,
                ImageUrl = user.ImageUrl,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PasswordHash = password,
                Email = user.Email,
                NormalizedEmail = user.Email,
                LockoutEnabled = true,
                LockoutEnd = user.IsLockedOut ? DateTime.MaxValue : null
            };
            var result = await _userManager.CreateAsync(obj, password);

            if (result.Succeeded)
            {
                var roleId = user.RoleId;
                if (roleId != null)
                {
                    user.Role = await _roleManager.FindByIdAsync(roleId);
                    if (user.Role != null)
                    {
                        await _userManager.AddToRoleAsync(obj, user.Role?.Name);
                    }
                    else
                    {
                        if (!_roleManager.RoleExistsAsync(SD.Client).GetAwaiter().GetResult())
                        {
                            await _roleManager.CreateAsync(new IdentityRole(SD.Client));
                        }
                        await _userManager.AddToRoleAsync(obj, SD.Client);
                    }
                }
                else
                {
                    if (!_roleManager.RoleExistsAsync(SD.Client).GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Client));
                    }
                    await _userManager.AddToRoleAsync(obj, SD.Client);
                }
                return _context.Users.FirstOrDefault(u => u.UserName == user.Email);
            }
            else
            {
                var mesg = result.Errors;
            }
            return null;
        }

        public async Task<List<User>> GetAllAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                user.IsLockedOut = await _userManager.IsLockedOutAsync(user);
                user.Role = await GetRoleByUserId(user);
            }
            return users;
        }
        public async Task<IdentityRole> GetRoleByUserId(User user)
        {
            try
            {
                if (user != null)
                {
                    var roleIds = await _userManager.GetRolesAsync(user);
                    var roleName = roleIds.FirstOrDefault();
                    if (!string.IsNullOrEmpty(roleName))
                    {
                        var role = await _roleManager.FindByNameAsync(roleName);
                        return role;
                    }
                    return null;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> RemoveAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }
}
