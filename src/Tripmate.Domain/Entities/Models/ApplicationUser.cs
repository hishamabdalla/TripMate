using Microsoft.AspNetCore.Identity;

namespace Tripmate.Domain.Entities.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Country { get; set; }
        public bool IsActive { get; set; } = false;
        public bool IsDeleted { get; set; }
        public string? VerificationCode { get; set; }
        public DateTime? VerificationCodeExpiration { get; set; }
        public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    }
  
}
