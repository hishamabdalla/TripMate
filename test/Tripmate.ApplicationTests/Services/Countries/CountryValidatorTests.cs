using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Tripmate.Application.Services.Countries;
using Tripmate.Application.Services.Countries.DTOs;
using Xunit;

namespace Tripmate.ApplicationTests.Services.Countries
{
    public class CountryValidatorTests
    {
        private readonly CountryValidator _countryValidator = new();

        #region Valid Test Cases

        [Theory]
        [InlineData("Test Country", "A valid description", "test.jpg", 1024 * 1024)] // 1 MB
        [InlineData("A", "Short description", "image.jpeg", 500 * 1024)] // 500 KB
        public void SetCountryDto_WithValidData_ShouldPassValidation(
            string name, 
            string description, 
            string fileName, 
            long fileSize)
        {
            // Arrange
            var mockFile = CreateMockFile(fileName, fileSize);
            var dto = new SetCountryDto
            {
                Name = name,
                Description = description,
                ImageUrl = mockFile.Object
            };

            // Act
            var result = _countryValidator.Validate(dto);

            // Assert
            result.IsValid.Should().BeTrue($"because all data is valid for {name}");
        }

        #endregion

        #region Invalid Test Cases - Name Validation

        [Theory]
        [InlineData(null, "Country name is required.")]
        [InlineData("", "Country name is required.")]
        [InlineData("   ", "Country name is required.")]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "Country name must not exceed 50 characters.")] // 51 chars
        public void SetCountryDto_WithInvalidName_ShouldFailValidation(string name, string expectedMessage)
        {
            // Arrange
            var mockFile = CreateMockFile("test.jpg", 1024 * 1024);
            var dto = new SetCountryDto
            {
                Name = name,
                Description = "Valid description",
                ImageUrl = mockFile.Object
            };

            // Act
            var result = _countryValidator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e =>
                e.PropertyName == nameof(SetCountryDto.Name) &&
                e.ErrorMessage.Contains(expectedMessage));
        }


        #endregion

        #region Invalid Test Cases - Description Validation

        [Theory]
        [InlineData(null,0, "Country Description is required.")]
        [InlineData("",0, "Country Description is required.")]
        [InlineData("a",550, "Country description must not exceed 500 characters.")]

        public void SetCountryDto_WithInvalidDescription_ShouldFailValidation(string? description,int descriptionLength, string expectedMessage)
        {
            // Arrange
            var mockFile = CreateMockFile("test.jpg", 1024 * 1024);
            var dto = new SetCountryDto
            {
                Name = "Valid Country",
                Description = new string('a', descriptionLength),
                ImageUrl = mockFile.Object
            };

            // Act
            var result = _countryValidator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse(expectedMessage);
            result.Errors.Should().Contain(e => 
                e.PropertyName == nameof(SetCountryDto.Description) && 
                e.ErrorMessage.Contains(expectedMessage));
        }

        #endregion

        #region Invalid Test Cases - Image Validation

      
        [Theory]
        [InlineData(null, 1024, "Image is required.")]
        [InlineData("test.txt", 1024, "Image must be one of the following formats")]
        [InlineData("test.jpg", 5 * 1024 * 1024, "Image size must not exceed 2 MB")]

        public void SetCountryDto_WithInvalidImage_ShouldFailValidation(string? fileName, long size, string expectedMessage)
        {
            // Arrange
            var mockFile = fileName== null ? null: CreateMockFile(fileName, size);
            var dto = new SetCountryDto
            {
                Name = "Valid Country",
                Description = "Valid description",
                ImageUrl = mockFile?.Object
            };

            // Act
            var result = _countryValidator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse(expectedMessage);
            result.Errors.Should().Contain(e =>
                e.PropertyName == nameof(SetCountryDto.ImageUrl));
        }

        #endregion

        #region Helper Methods

        private static Mock<IFormFile> CreateMockFile(string fileName, long fileSize)
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(fileSize);
            return mockFile;
        }

        #endregion
    }
}