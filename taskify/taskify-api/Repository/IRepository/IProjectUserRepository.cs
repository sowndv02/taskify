using taskify_api.Models;

namespace taskify_api.Repository.IRepository
{
    public interface IProjectUserRepository : IRepository<ProjectUser>
    {
        Task<ProjectUser> UpdateAsync(ProjectUser entity);
    }
}
