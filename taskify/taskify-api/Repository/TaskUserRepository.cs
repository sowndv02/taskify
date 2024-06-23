using taskify_api.Data;
using taskify_api.Models;
using taskify_api.Repository.IRepository;

namespace taskify_api.Repository
{
    public class TaskUserRepository : Repository<TaskUser>, ITaskUserRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<TaskUser> UpdateAsync(TaskUser entity)
        {
            try
            {
                _context.TaskUsers.Update(entity);
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
