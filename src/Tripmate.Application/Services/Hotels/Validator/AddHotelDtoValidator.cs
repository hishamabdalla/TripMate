using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Hotels.DTOS;
using Tripmate.Application.Services.Image;

namespace Tripmate.Application.Services.Hotels.Validator
{
    public class AddHotelDtoValidator:AbstractValidator<AddHotelDto>
    {
        public AddHotelDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Hotel name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
            RuleFor(x => x.Stars).InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
            RuleFor(x => x.RegionId).GreaterThan(0).WithMessage("RegionId must be greater than 0.");
            RuleFor(x => x.ImageUrl).Must(file => ImageValidator.BeValidImage(file, true))
                .WithMessage(ImageValidator.GetErrorMessage(true));
        }
    }
}
