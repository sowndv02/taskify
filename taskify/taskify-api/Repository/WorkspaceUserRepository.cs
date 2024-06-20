using taskify_api.Data;
using taskify_api.Models;
using taskify_api.Repository.IRepository;

namespace taskify_api.Repository
{
    public class WorkspaceUserRepository : Repository<WorkspaceUser>, IWorkspaceUserRepository
    {
        private readonly ApplicationDbContext _context;

        public WorkspaceUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<WorkspaceUser> UpdateAsync(WorkspaceUser entity)
        {
            try
            {
                _context.WorkspaceUsers.Update(entity);
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
