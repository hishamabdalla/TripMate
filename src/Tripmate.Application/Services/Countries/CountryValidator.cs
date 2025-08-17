using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Countries.DTOs;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Countries
{
    public class CountryValidator : AbstractValidator<SetCountryDto>
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
        private const long MaxSize = 2 * 1024 * 1024;

        public CountryValidator()
        {
            RuleFor(country => country.Name)
                .NotEmpty().WithMessage("Country name is required.")
                .MaximumLength(100).WithMessage("Country name must not exceed 100 characters.");
            
            RuleFor(country=>country.Description)
                .MaximumLength(500).WithMessage("Country description must not exceed 500 characters.");


            RuleFor(country => country.ImageUrl)
                .Must(F => _allowedExtensions.Contains(Path.GetExtension(F.FileName).ToLower()))
                .WithMessage($"Image must be one of the following formats: {string.Join(", ", _allowedExtensions)}")
                .Must(F => F.Length <= MaxSize)
                .WithMessage($"Image size must not exceed {MaxSize / 1024 / 1024} MB.");


        }
    }
      
}
