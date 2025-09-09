using System.ComponentModel.DataAnnotations;

namespace Tripmate.Application.Services.Identity.Login.DTOs
{
    public record LoginDto
    {
        [EmailAddress(ErrorMessage = "Invalid Email format")]
        public string Email { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        
    }
}
