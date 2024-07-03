using taskify_api.Models;
using taskify_api.Models.DTO;

namespace taskify_api.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string username);
        Task<TokenDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO);
        Task<TokenDTO> RefreshAccessToken(TokenDTO tokenDTO);
        Task RevokeRefreshToken(TokenDTO tokenDTO);
        Task<User> GetAsync(string userId);
        Task<User> UpdateAsync(User user);
        Task<User> CreateAsync(User user, string password);
        Task<bool> RemoveAsync(string userId);
        Task<List<User>> GetAllAsync();

    }
}
