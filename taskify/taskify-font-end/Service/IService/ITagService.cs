using taskify_font_end.Models.DTO;

namespace taskify_font_end.Service.IService
{
    public interface ITagService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(TagDTO dto);
        Task<T> UpdateAsync<T>(TagDTO dto);
        Task<T> DeleteAsync<T>(int id);
        Task<T> GetByUserIdAsync<T>(string userId);
        Task<T> GetByColorIdAsync<T>(int colorId);
    }
}
