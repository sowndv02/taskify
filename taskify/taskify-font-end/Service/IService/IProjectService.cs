using taskify_font_end.Models.DTO;

namespace taskify_font_end.Service.IService
{
    public interface IProjectService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(ProjectDTO dto);
        Task<T> UpdateAsync<T>(ProjectDTO dto);
        Task<T> DeleteAsync<T>(int id);
        Task<T> GetByUserIdAsync<T>(string userId);
        Task<T> GetByUserIdAndWorkspaceIdAsync<T>(string userId, int workspaceId);
        Task<T> GetByStatusIdAsync<T>(int statusId);
    }
}
