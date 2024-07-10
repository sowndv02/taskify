using taskify_api.Data;
using taskify_api.Models;
using taskify_api.Repository.IRepository;

namespace taskify_api.Repository
{
    public class MeetingUserRepository : Repository<MeetingUser>, IMeetingUserRepository
    {
        private readonly ApplicationDbContext _context;

        public MeetingUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<MeetingUser> UpdateAsync(MeetingUser entity)
        {
            try
            {
                _context.MeetingUsers.Update(entity);
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
