using taskify_api.Models;

namespace taskify_api.Repository.IRepository
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<Project> UpdateAsync(Project entity);
    }
}
