using taskify_api.Models;

namespace taskify_api.Repository.IRepository
{
    public interface INoteRepository : IRepository<Note>
    {
        Task<Note> UpdateAsync(Note entity);
    }
}
