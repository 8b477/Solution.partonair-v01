using System.Security.Claims;

namespace API.partonair_v01.Token
{

    public class HttpContextAccesor(IHttpContextAccessor httpContextAccessor)
        : IHttpContextAccesor
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public string? UserId()
        {
            var result = _httpContextAccessor.HttpContext?
                .User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            return string.IsNullOrEmpty(result) ? null : result;
        }

        public string? Role()
        {
            var result = _httpContextAccessor.HttpContext?
                .User
                .FindFirstValue(ClaimTypes.Role);

            return string.IsNullOrEmpty(result) ? null : result;
        }
    }
}