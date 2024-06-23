using taskify_api.Data;
using taskify_api.Models;
using taskify_api.Repository.IRepository;

namespace taskify_api.Repository
{
    public class PriorityRepository : Repository<Priority>, IPriorityRepository
    {
        private readonly ApplicationDbContext _context;

        public PriorityRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Priority> UpdateAsync(Priority entity)
        {
            try
            {
                _context.Priorities.Update(entity);
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
