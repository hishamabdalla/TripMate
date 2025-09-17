using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Restaurants.DTOS;

namespace Tripmate.Application.Services.Restaurants
{
    public class AddRestaurantDtoValidator : AbstractValidator<AddRestaurantDto>
    {
        public AddRestaurantDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Restaurant name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.CuisineType)
                .NotEmpty().WithMessage("Cuisine type is required.")
                .MaximumLength(50).WithMessage("Cuisine type cannot exceed 50 characters.");

            RuleFor(x => x.RegionId)
                .GreaterThan(0).WithMessage("RegionId must be greater than 0.");

            RuleFor(x => x.ImageUrl)
                .NotNull().WithMessage("Image is required.")
                .Must(file => file.Length > 0).WithMessage("Image cannot be empty.")
                .Must(file => file.ContentType.StartsWith("image/"))
                .WithMessage("Only image files are allowed.");
        }
    }
}
