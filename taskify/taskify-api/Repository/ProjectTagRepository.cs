using taskify_api.Data;
using taskify_api.Models;
using taskify_api.Repository.IRepository;

namespace taskify_api.Repository
{
    public class ProjectTagRepository : Repository<ProjectTag>, IProjectTagRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectTagRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<ProjectTag> UpdateAsync(ProjectTag entity)
        {
            try
            {
                _context.ProjectTags.Update(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
