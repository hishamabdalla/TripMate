using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tripmate.Application.Services.Identity.VerifyEmail.DTOs
{
    public class VerifyEmailDto
    {
        public string Email { get; set; }
        public string VerificationCode { get; set; }
    }
}
