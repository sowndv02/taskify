using taskify_api.Models;

namespace taskify_api.Repository.IRepository
{
    public interface IProjectTagRepository : IRepository<ProjectTag>
    {
        Task<ProjectTag> UpdateAsync(ProjectTag entity);
    }
}
