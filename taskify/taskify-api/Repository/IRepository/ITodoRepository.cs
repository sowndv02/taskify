using taskify_api.Models;

namespace taskify_api.Repository.IRepository
{
    public interface ITodoRepository : IRepository<Todo>
    {
        Task<Todo> UpdateAsync(Todo entity);
    }
}
