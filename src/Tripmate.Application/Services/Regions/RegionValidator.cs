using FluentValidation;
using Tripmate.Application.Services.Regions.DTOs;

namespace Tripmate.Application.Services.Regions
{
    public class RegionValidator:AbstractValidator<SetRegionDto>
    {
        private readonly string[] _allowedExcetention = new[] { ".jpg", ".jpeg", ".png" };
        private readonly int _maxSize= 2 * 1024 * 1024;
        public RegionValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Region name cannot be empty.");
            RuleFor(x => x.Name).MaximumLength(50).WithMessage("Region name cannot exceed 50 characters.");
            RuleFor(x => x.Description).MaximumLength(200).WithMessage("Region description cannot exceed 200 characters.");

            RuleFor(x => x.ImageUrl).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Image file is required.")
                .Must(file => _allowedExcetention.Contains(Path.GetExtension(file.FileName).ToLower()))
                .WithMessage($"Only the following types are allowed: {string.Join(", ", _allowedExcetention)}")
                .Must(file => file.Length <= _maxSize)
                .WithMessage($"File size must not exceed {_maxSize / (1024 * 1024)} MB.");
        }
        }
}
