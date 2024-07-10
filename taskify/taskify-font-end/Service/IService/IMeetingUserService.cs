using taskify_font_end.Models.DTO;

namespace taskify_font_end.Service.IService
{
    public interface IMeetingUserService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(MeetingUserDTO dto);
        Task<T> UpdateAsync<T>(MeetingUserDTO dto);
        Task<T> DeleteAsync<T>(int id);
        Task<T> DeleteByMeetingAndUserAsync<T>(int meetingId, string userId);
        Task<T> GetByUserIdAsync<T>(string userId);
        Task<T> GetByMeetingIdAsync<T>(int meetingId);
    }
}
