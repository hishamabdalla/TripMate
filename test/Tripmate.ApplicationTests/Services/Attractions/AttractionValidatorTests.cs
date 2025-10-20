using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Tripmate.Application.Services.Attractions.DTOs;
using Tripmate.Domain.Enums;
using Xunit;

namespace Tripmate.Application.Services.Attractions.Tests
{
    public class AttractionValidatorTests
    {
        private readonly AttractionValidator _attractionValidator = new();

        #region Valid Test Cases
        [Fact]
        public void SetAttractionDto_WithValidData_ShouldPassValidation()
        {
            // Arrange
            var mockFile = CreateMockFile("test.jpg", 1024 * 1024); // 1 MB
            var dto = new SetAttractionDto
            {
                Name = "Test Attraction",
                Description = "A valid description",
                Type = AttractionType.Museum,
                ImageUrl = mockFile.Object
            };
            // Act
            var result = _attractionValidator.Validate(dto);
            // Assert
            result.IsValid.Should().BeTrue("because all data is valid");
        }
        #endregion

        #region Invalid Test Cases - Name Validation

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // 51 chars
        public void SetAttractionDto_WithInvalidName_ShouldFailValidation(string name)
        {
            // Arrange
            var mockFile = CreateMockFile("test.jpg", 1024 * 1024);
            var dto = new SetAttractionDto
            {
                Name = name,
                Description = "Valid description",
                Type = AttractionType.Museum,
                ImageUrl = mockFile.Object
            };
            // Act
            var result = _attractionValidator.Validate(dto);
            // Assert
            result.IsValid.Should().BeFalse($"because the name '{name}' is invalid");
            
        }
        #endregion

        #region Invalid Test Cases - Description Validation
        [Fact]
        public void SetAttractionDto_WithTooLongDescription_ShouldFailValidation()
        {
            // Arrange
            var longDescription = new string('a', 1001); // 1001 chars
            var mockFile = CreateMockFile("test.jpg", 1024 * 1024);
            var dto = new SetAttractionDto
            {
                Name = "Valid Name",
                Description = longDescription,
                Type = AttractionType.Museum,
                ImageUrl = mockFile.Object
            };
            // Act
            var result = _attractionValidator.Validate(dto);
            // Assert
            result.IsValid.Should().BeFalse("because the description exceeds the maximum length");
        
        }
        #endregion

        //#region Invalid Test Cases - Type Validation

        //[Theory]
        //[InlineData(null)]
        //[InlineData("")]
        //[InlineData("InvalidType")]
        //[InlineData("123")]
        //public void SetAttractionDto_WithInvalidType_ShouldFailValidation(string type)
        //{
        //    // Arrange
        //    var mockFile = CreateMockFile("test.jpg", 1024 * 1024);
        //    var dto = new SetAttractionDto
        //    {
        //        Name = "Valid Name",
        //        Description = "Valid description",
        //        Type = nul
        //        ImageUrl = mockFile.Object
        //    };
        //    // Act
        //    var result = _attractionValidator.Validate(dto);
        //    // Assert
        //    result.IsValid.Should().BeFalse($"because the type '{type}' is invalid");
        //}
        //#endregion

        #region Invalid Test Cases - Image Validation
        [Theory]
        [InlineData(null, 1024 * 1024)] // Null file
        [InlineData("test.txt", 1024 * 1024)] // Invalid extension
        [InlineData("test.jpg", 3 * 1024 * 1024)] // Exceeds size limit
        public void SetAttractionDto_WithInvalidImage_ShouldFailValidation(string fileName, int fileSize)
        {
            // Arrange
            var mockFile = fileName != null ? CreateMockFile(fileName, fileSize) : null;
            var dto = new SetAttractionDto
            {
                Name = "Valid Name",
                Description = "Valid description",
                Type = AttractionType.Safari,
                ImageUrl = mockFile?.Object
            };
            // Act
            var result = _attractionValidator.Validate(dto);
            // Assert
            result.IsValid.Should().BeFalse("because the image is invalid");
        }
        #endregion

        #region Helper Methods
        private static Mock<IFormFile> CreateMockFile(string fileName, int fileSize)
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(fileSize);
            return mockFile;

        }
        #endregion
    }
}