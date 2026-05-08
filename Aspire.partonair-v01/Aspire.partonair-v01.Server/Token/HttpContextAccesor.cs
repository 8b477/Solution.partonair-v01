using System.Security.Claims;

namespace API.partonair_v01.Token
{
    public class HttpContextAccesor(IHttpContextAccessor httpContextAccessor) : IHttpContextAccesor
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public string? UserId()
        {
            var result = _httpContextAccessor.HttpContext?.User.FindFirstValue("sub");
            return string.IsNullOrEmpty(result) ? null : result;
        }

        public string? Role()
        {
            var result = _httpContextAccessor.HttpContext?.User.FindFirstValue("role");
            return string.IsNullOrEmpty(result) ? null : result;
        }
    }
}
