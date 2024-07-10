using taskify_font_end.Models.DTO;

namespace taskify_font_end.Service.IService
{
    public interface IMeetingService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(MeetingDTO dto);
        Task<T> UpdateAsync<T>(MeetingDTO dto);
        Task<T> DeleteAsync<T>(int id);
        Task<T> GetByUserIdAsync<T>(string userId);
        Task<T> GetByWorkspaceIdAsync<T>(int workspaceId);
        Task<T> GetByUserIdAndWorkspaceIdAsync<T>(string userId, int workspaceId);
    }
}
