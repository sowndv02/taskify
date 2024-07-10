using taskify_api.Data;
using taskify_api.Models;
using taskify_api.Repository.IRepository;

namespace taskify_api.Repository
{
    public class MeetingRepository : Repository<Meeting>, IMeetingRepository
    {
        private readonly ApplicationDbContext _context;

        public MeetingRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Meeting> UpdateAsync(Meeting entity)
        {
            try
            {
                _context.Meetings.Update(entity);
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
