using taskify_api.Data;
using taskify_api.Models;
using taskify_api.Repository.IRepository;

namespace taskify_api.Repository
{
    public class ActivityTypeRepository : Repository<ActivityType>, IActivityTypeRepository
    {

        private readonly ApplicationDbContext _context;

        public ActivityTypeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<ActivityType> UpdateAsync(ActivityType entity)
        {
            try
            {
                _context.ActivityTypes.Update(entity);
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
