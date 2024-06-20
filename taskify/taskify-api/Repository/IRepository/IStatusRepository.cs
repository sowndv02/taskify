using taskify_api.Models;

namespace taskify_api.Repository.IRepository
{
    public interface IStatusRepository : IRepository<Status>
    {
        Task<Status> UpdateAsync(Status entity);
    }
}
