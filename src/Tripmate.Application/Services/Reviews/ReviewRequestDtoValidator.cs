using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Reviews.DTOs;


namespace Tripmate.Application.Reviews
{
    public class ReviewRequestDtoValidator:AbstractValidator<ReviewRequestDto>
    {
        public ReviewRequestDtoValidator()
        {
            RuleFor(x=>x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
            RuleFor(x=>x.Comment)
                .NotEmpty().WithMessage("Comment is required.");
        }

    }
}
