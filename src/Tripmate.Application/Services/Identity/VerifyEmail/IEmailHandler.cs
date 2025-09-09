namespace Tripmate.Application.Services.Identity.VerifyEmail
{
    public interface IEmailHandler
    {
         Task SendVerificationEmail(string email, string verificationCode);
         Task SendResetCodeEmail(string email, string code);
        
    }
}
