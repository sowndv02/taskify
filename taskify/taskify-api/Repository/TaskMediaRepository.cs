using taskify_api.Data;
using taskify_api.Models;
using taskify_api.Repository.IRepository;

namespace taskify_api.Repository
{
    public class TaskMediaRepository : Repository<TaskMedia>, ITaskMediaRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskMediaRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<TaskMedia> UpdateAsync(TaskMedia entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _context.TaskMedias.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
