using taskify_font_end.Models.DTO;

namespace taskify_font_end.Service.IService
{
    public interface ITaskUserService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(TaskUserDTO dto);
        Task<T> UpdateAsync<T>(TaskUserDTO dto);
        Task<T> DeleteAsync<T>(int id);
        Task<T> GetByUserIdAsync<T>(string userId);
        Task<T> GetByTaskIdAsync<T>(int id);
    }
}
