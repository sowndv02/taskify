using taskify_api.Data;
using taskify_api.Models;
using taskify_api.Repository.IRepository;

namespace taskify_api.Repository
{
    public class NoteRepository : Repository<Note>, INoteRepository
    {
        private readonly ApplicationDbContext _context;

        public NoteRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Note> UpdateAsync(Note entity)
        {
            try
            {
                _context.Notes.Update(entity);
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
