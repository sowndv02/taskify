using taskify_font_end.Models.DTO;

namespace taskify_font_end.Service.IService
{
    public interface ITaskService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(TaskDTO dto);
        Task<T> UpdateAsync<T>(TaskDTO dto);
        Task<T> DeleteAsync<T>(int id);
        Task<T> GetByUserIdAsync<T>(string userId);
        Task<T> GetByStatusIdAsync<T>(int statusId);
        Task<T> GetByStatusIdAndProjectIdAsync<T>(int projectId, int statusId);
        Task<T> GetByProjectIdAsync<T>(int projectId);
        
    }
}
