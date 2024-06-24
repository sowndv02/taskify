using taskify_font_end.Models.DTO;

namespace taskify_font_end.Service.IService
{
    public interface IWorkspaceService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(WorkspaceDTO dto);
        Task<T> UpdateAsync<T>(WorkspaceDTO dto);
        Task<T> DeleteAsync<T>(int id);
    }
}
