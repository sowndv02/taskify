using taskify_font_end.Models.DTO;

namespace taskify_font_end.Service.IService
{
    public interface IRoleService
    {
        Task<T> GetByUserIdAsync<T>(string userId);
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(string id);
        Task<T> CreateAsync<T>(RoleDTO dto);
        Task<T> UpdateAsync<T>(RoleDTO dto);
        Task<T> DeleteAsync<T>(int id);
    }
}
