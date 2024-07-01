using taskify_api.Data;
using taskify_api.Models;
using taskify_api.Repository.IRepository;

namespace taskify_api.Repository
{
    public class ProjectMediaRepository : Repository<ProjectMedia>, IProjectMediaRepository
    {
        private readonly ApplicationDbContext _context;
        public ProjectMediaRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<ProjectMedia> UpdateAsync(ProjectMedia entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _context.ProjectMedias.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
