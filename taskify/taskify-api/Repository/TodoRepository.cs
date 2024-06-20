using taskify_api.Data;
using taskify_api.Models;
using taskify_api.Repository.IRepository;

namespace taskify_api.Repository
{
    public class TodoRepository : Repository<Todo>, ITodoRepository
    {
        private readonly ApplicationDbContext _context;

        public TodoRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Todo> UpdateAsync(Todo entity)
        {
            try
            {
                _context.Todos.Update(entity);
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
