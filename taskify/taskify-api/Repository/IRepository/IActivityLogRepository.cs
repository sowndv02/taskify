using taskify_api.Models;

namespace taskify_api.Repository.IRepository
{
    public interface IActivityLogRepository : IRepository<ActivityLog>
    {
        Task<ActivityLog> UpdateAsync(ActivityLog entity);
    }
}
