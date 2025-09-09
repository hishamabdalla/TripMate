using Microsoft.EntityFrameworkCore;

namespace Tripmate.Domain.Entities.Models
{
    [Owned]
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public bool IsExpired() => DateTime.UtcNow >= Expiration;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedOn { get; set; }
        public bool IsActive => RevokedOn == null && !IsExpired();
        
    }
}
