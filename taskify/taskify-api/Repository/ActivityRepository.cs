using taskify_api.Data;
using taskify_api.Models;
using taskify_api.Repository.IRepository;

namespace taskify_api.Repository
{
    public class ActivityRepository : Repository<Activity>, IActivityRepository
    {

        private readonly ApplicationDbContext _context;

        public ActivityRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Activity> UpdateAsync(Activity entity)
        {
            try
            {
                _context.Activities.Update(entity);
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
