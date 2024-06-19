using taskify_api.Data;
using taskify_api.Models;
using taskify_api.Repository.IRepository;

namespace taskify_api.Repository
{
    public class WorkspaceRepository : Repository<Workspace>, IWorkspaceRepository
    {

        private readonly ApplicationDbContext _context;

        public WorkspaceRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Workspace> UpdateAsync(Workspace entity)
        {
            try
            {
                _context.Workspaces.Update(entity);
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
