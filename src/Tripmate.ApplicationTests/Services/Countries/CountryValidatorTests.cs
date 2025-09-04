using Xunit;
using Tripmate.Application.Services.Countries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Countries.DTOs;
using Microsoft.AspNetCore.Http;
using Moq;
using FluentAssertions;

namespace Tripmate.Application.Services.Countries.Tests
{
    public class CountryValidatorTests
    {

        private readonly CountryValidator _countryValidator;
        public CountryValidatorTests()
        {
            _countryValidator = new CountryValidator();
        }

        [Fact]
        public void SetCountryDto_WithValidData_ShouldPassValidation()
        {
            // Arrange
            var moqFile = new Mock<IFormFile>();
            moqFile.Setup(f => f.FileName).Returns("test.jpg");
            moqFile.Setup(f => f.Length).Returns(1024 * 1024); // 1 MB

            var dto = new SetCountryDto
            {
                Name = "Test Country",
                Description = "A description for the test country",
                ImageUrl = moqFile.Object

            };

            // Act
            var result = _countryValidator.Validate(dto);

            // Assert

            result.IsValid.Should().BeTrue(" because all data is valid");

        }


        [Fact]
        public void SetCountryDto_WithInvalidData_ShouldFailValidation()
        {
            // Arrange
            var moqFile = new Mock<IFormFile>();
            moqFile.Setup(f => f.FileName).Returns("test.txt"); // Invalid extension
            moqFile.Setup(f => f.Length).Returns(3 * 1024 * 1024); // 3 MB, exceeds limit
            var dto = new SetCountryDto
            {
                Name = "", // Empty name
                Description = new string('a', 600), // Exceeds max length
                ImageUrl = moqFile.Object
            };
            // Act
            var result = _countryValidator.Validate(dto);
            // Assert
            result.IsValid.Should().BeFalse(" because the data contains multiple validation errors");
            result.Errors.Should().HaveCount(3, " because there are four validation issues");

        }

        [Fact]
        public void SetCountryDto_WithNullName_ShouldFailValidation()
        {
           
            // Arrange
            var moqFile = new Mock<IFormFile>();
            moqFile.Setup(f => f.FileName).Returns("test.jpg");
            moqFile.Setup(f => f.Length).Returns(1024 * 1024); // 1 MB
            var dto = new SetCountryDto
            {
                Name = null, // Null name
                Description = "A valid description",
                ImageUrl = moqFile.Object
            };
            // Act
            var result = _countryValidator.Validate(dto);
            // Assert
            result.IsValid.Should().BeFalse(" because the name is null");
        }

        [Fact]
        public void SetCountryDto_WithNameExceeds50Characters_ShouldFailValidation()
        {
            //Arrange
            var moqFile = new Mock<IFormFile>();
            moqFile.Setup(f => f.FileName).Returns("test.jpg");
            moqFile.Setup(f => f.Length).Returns(1024 * 1024); // 1 MB

            var dto = new SetCountryDto
            {
                Name = new string('a', 51),
                Description = "A valid description",
                ImageUrl = moqFile.Object


            };

            //Act

            var result = _countryValidator.Validate(dto);

            //Assert
            result.IsValid.Should().BeFalse(" because the name exceeds 50 characters");

        }
    }
}