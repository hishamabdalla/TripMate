using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Tripmate.Application.Services.Identity.VerifyEmail
{
    public class EmailHandler : IEmailHandler
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailHandler> _logger;
        public EmailHandler(IConfiguration configuration, ILogger<EmailHandler> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task SendResetCodeEmail(string email, string code)
        {
            try
            {
                var emailSetting = _configuration.GetSection("EmailSettings");
                if (string.IsNullOrEmpty(emailSetting["Email"]))
                    throw new ArgumentNullException("Email is not configured");
                if (string.IsNullOrEmpty(emailSetting["DisplayName"]))
                    throw new ArgumentNullException("Name is not configured");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    emailSetting["DisplayName"],
                    emailSetting["Email"]));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = "Reset Your Password";
                message.Body = new TextPart(TextFormat.Html)
                {
                    Text =EmailTemplates.GetResetPasswordTemplate(code)
                };
                using var smtp = new SmtpClient();
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await smtp.ConnectAsync(
                    emailSetting["SmtpServer"],
                    int.Parse(emailSetting["SmtpPort"]),
                    SecureSocketOptions.StartTls);

                if (smtp.Capabilities.HasFlag(MailKit.Net.Smtp.SmtpCapabilities.Authentication))
                {
                    await smtp.AuthenticateAsync(emailSetting["Email"], emailSetting["Password"]);
                }

                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"Password reset email sent to {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send reset email to {email}");
                throw;
            }
        }

        public async Task SendVerificationEmail(string email, string verificationCode)
        {
            try
            {
                var emailSetting = _configuration.GetSection("EmailSettings");
                if (string.IsNullOrEmpty(emailSetting["Email"]))
                    throw new ArgumentNullException("Email is not configured");
                if (string.IsNullOrEmpty(emailSetting["DisplayName"]))
                    throw new ArgumentNullException("Name is not configured");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    emailSetting["DisplayName"],
                    emailSetting["Email"]));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = "Your Email Verification Code";

                message.Body = new TextPart(TextFormat.Html)
                {
                    Text =EmailTemplates.VerifyEmailTemplate(verificationCode)
                };
                using var client = new SmtpClient();
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Timeout = 30000;

                await client.ConnectAsync(
                emailSetting["SmtpServer"],
                int.Parse(emailSetting["SmtpPort"]),
                MailKit.Security.SecureSocketOptions.StartTls);

                if (client.Capabilities.HasFlag(SmtpCapabilities.Authentication))
                {
                    var username = emailSetting["Email"];
                    var password = emailSetting["Password"];
                    await client.AuthenticateAsync(emailSetting["Email"],
                    emailSetting["Password"]);
                }
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                _logger.LogInformation($"Verification email successfully delivered to {email}");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"Email delivery failed for recipient: {email}");
                throw;
            }
        }
    }
}
