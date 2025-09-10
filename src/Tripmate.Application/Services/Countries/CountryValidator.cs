using FluentValidation;
using Tripmate.Application.Services.Countries.DTOs;

namespace Tripmate.Application.Services.Countries
{
    public class CountryValidator : AbstractValidator<SetCountryDto>
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
        private const long MaxSize = 2 * 1024 * 1024;

        public CountryValidator()
        {
            RuleFor(country => country.Name)
                .NotEmpty().WithMessage(errorMessage: "Country name is required.")
                .MaximumLength(50).WithMessage("Country name must not exceed 50 characters.");
            
            RuleFor(country=>country.Description)
                 .NotEmpty().WithMessage(errorMessage: "Country Description is required.")
                .MaximumLength(500).WithMessage("Country description must not exceed 500 characters.");

            RuleFor(country => country.ImageUrl).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Image is required.")
                .Must(file => file != null && _allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
                .WithMessage($"Image must be one of the following formats: {string.Join(", ", _allowedExtensions)}")
                .Must(file => file != null && file.Length <= MaxSize)
                .WithMessage($"Image size must not exceed {MaxSize / 1024 / 1024} MB.");
        }
    }
}
