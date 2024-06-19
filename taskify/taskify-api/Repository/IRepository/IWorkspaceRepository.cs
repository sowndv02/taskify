using taskify_api.Models;

namespace taskify_api.Repository.IRepository
{
    public interface IWorkspaceRepository : IRepository<Workspace>
    {
        Task<Workspace> UpdateAsync(Workspace entity);
    }
}
