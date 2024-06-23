using taskify_api.Models;

namespace taskify_api.Repository.IRepository
{
    public interface ITaskUserRepository : IRepository<TaskUser>
    {
        Task<TaskUser> UpdateAsync(TaskUser entity);
    }
}
