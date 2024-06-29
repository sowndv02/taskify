using taskify_font_end.Models.DTO;

namespace taskify_font_end.Service.IService
{
    public interface IProjectUserService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(ProjectUserDTO dto);
        Task<T> UpdateAsync<T>(ProjectUserDTO dto);
        Task<T> DeleteAsync<T>(int id);
        Task<T> DeleteByProjectAndUserAsync<T>(int projectId, string userId);
        Task<T> GetByUserIdAsync<T>(string userId);
    }
}
