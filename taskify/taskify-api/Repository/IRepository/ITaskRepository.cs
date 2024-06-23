using taskify_api.Models;

namespace taskify_api.Repository.IRepository
{
    public interface ITaskRepository : IRepository<TaskModel>
    {
        Task<TaskModel> UpdateAsync(TaskModel entity);
    }
}
