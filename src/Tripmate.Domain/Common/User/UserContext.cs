using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Tripmate.Domain.Common.User
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<UserContext> _logger;

        public UserContext(IHttpContextAccessor contextAccessor, ILogger<UserContext> logger)
        {
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        public CurrentUser? GetCurrentUser()
        {
            var user = _contextAccessor.HttpContext?.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                _logger.LogWarning("User is not authenticated");
                return null;
            }

            var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
             var name = user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
            var email = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
            var role = user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

            return new CurrentUser(id, name, email, role);
        }
    }
}
