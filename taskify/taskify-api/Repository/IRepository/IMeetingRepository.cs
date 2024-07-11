using taskify_api.Models;

namespace taskify_api.Repository.IRepository
{
    public interface IMeetingRepository : IRepository<Meeting>
    {
        Task<Meeting> UpdateAsync(Meeting entity);
    }
}
