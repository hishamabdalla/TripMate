using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tripmate.Domain.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public string Country { get; set; }
        public bool IsActive { get; set; } = false;
        public bool IsEmailVerified { get; set; } = false;
        public string VerificationCode { get; set; }
        public DateTime? VerificationCodeExpiration { get; set; }

    }
}
