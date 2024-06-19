using taskify_api.Models;

namespace taskify_api.Repository.IRepository
{
    public interface ITagRepository : IRepository<Tag>
    {
        Task<Tag> UpdateAsync(Tag status);
    }
}
