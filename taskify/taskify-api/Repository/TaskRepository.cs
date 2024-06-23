using taskify_api.Data;
using taskify_api.Models;
using taskify_api.Repository.IRepository;

namespace taskify_api.Repository
{
    public class TaskRepository : Repository<TaskModel>, ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<TaskModel> UpdateAsync(TaskModel entity)
        {
            try
            {
                _context.Tasks.Update(entity);
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
