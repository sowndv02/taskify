using taskify_api.Models;

namespace taskify_api.Repository.IRepository
{
    public interface IActivityTypeRepository : IRepository<ActivityType>
    {
        Task<ActivityType> UpdateAsync(ActivityType entity);
    }
}
