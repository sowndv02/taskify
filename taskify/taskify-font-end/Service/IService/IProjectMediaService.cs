using taskify_font_end.Models.DTO;

namespace taskify_font_end.Service.IService
{
    public interface IProjectMediaService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(ProjectMediaDTO dto);
        Task<T> UpdateAsync<T>(ProjectMediaDTO dto);
        Task<T> DeleteAsync<T>(int id);
        Task<T> GetByUserIdAsync<T>(string userId);
        Task<T> GetByStatusIdAsync<T>(int statusId);
        Task<T> GetByStatusIdAndProjectIdAsync<T>(int projectId, int statusId);
        Task<T> GetByProjectIdAsync<T>(int projectId);
    }
}
