using taskify_font_end.Models.DTO;

namespace taskify_font_end.Service.IService
{
    public interface IUserService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(string id);
        Task<T> CreateAsync<T>(UserDTO dto);
        Task<T> UpdateAsync<T>(UserDTO dto);
        Task<T> DeleteAsync<T>(string id);
    }
}
