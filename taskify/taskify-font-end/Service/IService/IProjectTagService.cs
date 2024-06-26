using taskify_font_end.Models.DTO;

namespace taskify_font_end.Service.IService
{
    public interface IProjectTagService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(ProjectTagDTO dto);
        Task<T> UpdateAsync<T>(ProjectTagDTO dto);
        Task<T> DeleteAsync<T>(int id);
        Task<T> GetByUserIdAsync<T>(string userId);
    }
}
