using taskify_font_end.Models.DTO;

namespace taskify_font_end.Service.IService
{
    public interface ITodoService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(TodoDTO dto);
        Task<T> UpdateAsync<T>(TodoDTO dto);
        Task<T> DeleteAsync<T>(int id);
        Task<T> GetByUserIdAsync<T>(string userId);
    }
}
