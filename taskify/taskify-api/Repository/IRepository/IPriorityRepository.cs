using taskify_api.Models;

namespace taskify_api.Repository.IRepository
{
    public interface IPriorityRepository : IRepository<Priority>
    {
        Task<Priority> UpdateAsync(Priority entity);
    }
}
