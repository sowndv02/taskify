using taskify_api.Data;
using taskify_api.Models;
using taskify_api.Repository.IRepository;

namespace taskify_api.Repository
{
    public class ColorRepository : Repository<Color>, IColorRepository
    {
        private readonly ApplicationDbContext _context;

        public ColorRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Color> UpdateAsync(Color entity)
        {
            try
            {
                _context.Colors.Update(entity);
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
