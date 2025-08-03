using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Tripmate.Application.Services.Identity.VerifyEmail
{
    public interface IEmailHandler
    {
        public Task SendVerificationEmail(string email, string verificationCode);
    }
}
