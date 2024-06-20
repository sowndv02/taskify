using AutoMapper.Internal;
using taskify_font_end.Models;

namespace taskify_font_end.Service.IService
{
    public interface IBaseServices
    {
        APIResponse responseModel { get; set; }
        Task<T> SendAsync<T>(APIRequest apiRequest, bool withBearer = true);
    }
}
