using taskify_api.Models;

namespace taskify_api.Repository.IRepository
{
    public interface IActivityRepository : IRepository<Activity>
    {
        Task<Activity> UpdateAsync(Activity entity);
    }
}
