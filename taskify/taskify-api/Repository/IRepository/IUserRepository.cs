using taskify_api.Models;
using taskify_api.Models.DTO;

namespace taskify_api.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string username);
        Task<TokenDTO> Login(LoginRequestDTO loginRequestDTO, bool checkPassword = true);
        Task<User> Register(RegisterationRequestDTO registerationRequestDTO);
        Task<TokenDTO> RefreshAccessToken(TokenDTO tokenDTO);
        Task RevokeRefreshToken(TokenDTO tokenDTO);
        Task<User> GetAsync(string userId);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> LockoutUser(string userId);
        Task<User> UnlockUser(string userId);
        Task<User> UpdatePasswordAsync(UpdatePasswordRequestDTO updatePasswordRequestDTO);
        Task<User> UpdateAsync(User user);
        Task<User> CreateAsync(User user, string password);
        Task<bool> RemoveAsync(string userId);
        Task<List<User>> GetAllAsync();

    }
}
