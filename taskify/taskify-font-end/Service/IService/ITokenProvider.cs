using taskify_font_end.Models.DTO;

namespace taskify_font_end.Service.IService
{
    public interface ITokenProvider
    {
        void SetToken(TokenDTO tokenDTO);
        TokenDTO? GetToken();
        void ClearToken();
    }
}
