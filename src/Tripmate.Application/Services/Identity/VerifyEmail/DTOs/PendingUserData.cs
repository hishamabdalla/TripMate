namespace Tripmate.Application.Services.Identity.VerifyEmail.DTOs
{
    public class PendingUserData
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Password { get; set; } 
        public string Country { get; set; }
        public string VerificationCode { get; set; }
        public DateTime Expiration { get; set; }
    }
}
