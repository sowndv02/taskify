using taskify_api.Data;
using taskify_api.Models;
using taskify_api.Repository.IRepository;

namespace taskify_api.Repository
{
    public class ProjectUserRepository : Repository<ProjectUser>, IProjectUserRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<ProjectUser> UpdateAsync(ProjectUser entity)
        {
            try
            {
                _context.ProjectUsers.Update(entity);
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
