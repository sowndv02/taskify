using taskify_api.Models;

namespace taskify_api.Repository.IRepository
{
    public interface IProjectMediaRepository : IRepository<ProjectMedia>
    {
        Task<ProjectMedia> UpdateAsync(ProjectMedia entity);
    }
}
