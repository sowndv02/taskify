using taskify_api.Data;
using taskify_api.Models;
using taskify_api.Repository.IRepository;

namespace taskify_api.Repository
{
    public class MilestoneRepository : Repository<Milestone>, IMilestoneRepository
    {
        private readonly ApplicationDbContext _context;

        public MilestoneRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Milestone> UpdateAsync(Milestone entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _context.Milestones.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
