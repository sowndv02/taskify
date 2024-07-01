using taskify_api.Models;

namespace taskify_api.Repository.IRepository
{
    public interface ITaskMediaRepository : IRepository<TaskMedia>
    {
        Task<TaskMedia> UpdateAsync(TaskMedia entity);
    }
}
