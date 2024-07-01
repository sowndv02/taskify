using taskify_api.Models;

namespace taskify_api.Repository.IRepository
{
    public interface IMilestoneRepository : IRepository<Milestone>
    {
        Task<Milestone> UpdateAsync(Milestone entity);
    }
}
