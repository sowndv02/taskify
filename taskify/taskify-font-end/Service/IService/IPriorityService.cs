using taskify_font_end.Models.DTO;

namespace taskify_font_end.Service.IService
{
    public interface IPriorityService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(PriorityDTO dto);
        Task<T> UpdateAsync<T>(PriorityDTO dto);
        Task<T> DeleteAsync<T>(int id);
        Task<T> GetByUserIdAsync<T>(string userId);
    }
}
