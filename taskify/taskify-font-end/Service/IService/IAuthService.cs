using taskify_font_end.Models.DTO;

namespace taskify_font_end.Service.IService
{
    public interface IAuthService
    {
        Task<T> LoginAsync<T>(LoginRequestDTO objToCreate);
        Task<T> RegisterAsync<T>(RegisterationRequestDTO objToCreate);

        Task<T> LogoutAsync<T>(TokenDTO obj);
    }
}
