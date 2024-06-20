using taskify_api.Models;

namespace taskify_api.Repository.IRepository
{
    public interface IColorRepository : IRepository<Color>
    {
        Task<Color> UpdateAsync(Color entity);
    }
}
