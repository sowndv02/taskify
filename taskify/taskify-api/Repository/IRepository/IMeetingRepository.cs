using taskify_api.Models;

namespace taskify_api.Repository.IRepository
{
    public interface IMeetingRepository
    {
        Task<Meeting> UpdateAsync(Meeting entity, string token);
        Task<Meeting> GetAsync(string userId);
        Task<Meeting> CreateAsync(Meeting meeting, string token);
        Task<bool> RemoveAsync(int id);
        Task<List<User>> GetAllAsync();
    }
}
