using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tripmate.Application.Services.Identity.Register.DTOs
{
    public record RegisterDto
    {
        public string UserName { get; set; } = string.Empty;
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; } = null;
        [DataType(DataType.Password)]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "{0} must be between {2} and {1} characters ")]
        public string Password { get; set; } = string.Empty;
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        public string Country { get; set; } = string.Empty;

    }
}
