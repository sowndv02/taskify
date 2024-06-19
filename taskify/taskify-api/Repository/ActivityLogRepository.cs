using taskify_api.Data;
using taskify_api.Models;
using taskify_api.Repository.IRepository;

namespace taskify_api.Repository
{
    public class ActivityLogRepository : Repository<ActivityLog>, IActivityLogRepository
    {

        private readonly ApplicationDbContext _context;

        public ActivityLogRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<ActivityLog> UpdateAsync(ActivityLog entity)
        {
            try
            {
                _context.ActivityLogs.Update(entity);
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
