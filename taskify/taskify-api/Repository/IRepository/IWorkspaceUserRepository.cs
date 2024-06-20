using taskify_api.Models;

namespace taskify_api.Repository.IRepository
{
    public interface IWorkspaceUserRepository : IRepository<WorkspaceUser>
    {
        Task<WorkspaceUser> UpdateAsync(WorkspaceUser entity);

    }
}
