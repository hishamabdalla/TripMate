namespace Tripmate.Application.Services.Identity.VerifyEmail.DTOs
{
    public class VerifyEmailDto
    {
        public string Email { get; set; }
        public string VerificationCode { get; set; }
    }
}
