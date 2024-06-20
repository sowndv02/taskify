using taskify_font_end.Models;

namespace taskify_font_end.Service.IService
{
    public interface IApiMessageRequestBuilder
    {
        HttpRequestMessage Build(APIRequest apiRequest);
    }
}
