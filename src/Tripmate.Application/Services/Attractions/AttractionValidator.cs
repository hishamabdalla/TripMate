using FluentValidation;
using Tripmate.Application.Services.Attractions.DTOs;
using Tripmate.Domain.Enums;

namespace Tripmate.Application.Services.Attractions
{
    public class AttractionValidator:AbstractValidator<SetAttractionDto>
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
        private const long MaxSize = 2 * 1024 * 1024;
        public AttractionValidator()
        {
            RuleFor(attraction=>attraction.Name)
                .NotEmpty().WithMessage("Attraction name is required.")
                .MaximumLength(50).WithMessage("Attraction name must not exceed 50 characters.");

            RuleFor(attraction => attraction.Description)
                .MaximumLength(1000).WithMessage("Attraction description must not exceed 1000 characters.");

            RuleFor(attraction => attraction.Type)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Attraction type is required.")
                .IsInEnum().WithMessage("Invalid attraction type.");



            RuleFor(attraction => attraction.ImageUrl)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Image is required.")
                .Must(file => file != null && _allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
                .WithMessage($"Image must be one of the following formats:{string.Join(", ", _allowedExtensions)}")
                .Must(file => file != null && file.Length <= MaxSize)
                .WithMessage($"Image size must not exceed {MaxSize / 1024 / 1024} MB");





        }
    }
}
