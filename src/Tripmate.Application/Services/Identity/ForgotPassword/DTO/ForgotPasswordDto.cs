using System.ComponentModel.DataAnnotations;

namespace Tripmate.Application.Services.Identity.ForgotPassword.DTO
{
    public class ForgotPasswordDto
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
