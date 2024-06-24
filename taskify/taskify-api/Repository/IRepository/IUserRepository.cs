using taskify_api.Models;
using taskify_api.Models.DTO;

namespace taskify_api.Repository.IRepository
{
    public interface IUserRepository : IRepository<User>
    {
        bool IsUniqueUser(string username);
        Task<TokenDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO);
        Task<TokenDTO> RefreshAccessToken(TokenDTO tokenDTO);
        Task RevokeRefreshToken(TokenDTO tokenDTO);
        Task<User> UpdateAsync(User entity);

    }
}
