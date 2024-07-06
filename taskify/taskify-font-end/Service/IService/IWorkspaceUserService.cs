using taskify_font_end.Models.DTO;

namespace taskify_font_end.Service.IService
{
    public interface IWorkspaceUserService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetByWorkspaceIdAsync<T>(int id);
        Task<T> GetByUserIdAsync<T>(string userId);
        Task<T> CreateAsync<T>(WorkspaceUserDTO dto);
        Task<T> UpdateAsync<T>(WorkspaceUserDTO dto);
        Task<T> DeleteAsync<T>(int id);
        Task<T> DeleteByWorkspaceAndUserAsync<T>(int workspaceId, string userId);
    }
}
