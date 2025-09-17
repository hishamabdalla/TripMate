using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Countries;
using Tripmate.Application.Services.Countries.DTOs;
using Tripmate.Application.Services.Image;
using Tripmate.Domain.Entities.Models;
using Tripmate.Domain.Interfaces;
using Xunit;
using static System.Net.WebRequestMethods;

namespace Tripmate.Application.Services.Countries.Tests
{
    public class CountryServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<CountryService>> _loggerMock;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly CountryService _countryService;
        public CountryServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<CountryService>>();
            _fileServiceMock = new Mock<IFileService>();
            _countryService = new CountryService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _fileServiceMock.Object);
        }

        #region Valid Test Cases

        [Fact]
        public async Task AddAsync_ValidCountry_ReturnsApiResponse()
        {

            // Arrange
            var moqFile = new Mock<IFormFile>();
            moqFile.Setup(f => f.FileName).Returns("image.jpg");
            moqFile.Setup(f => f.Length).Returns(1024);

            var setCountryDto = new SetCountryDto
            {
                Name = "Test Country",
                ImageUrl = moqFile.Object // Assuming image upload is handled separately
            };
            var country = new Country
            {
                Id = 1,
                Name = "Test Country",
                ImageUrl = "http://example.com/image.jpg"
            };
            var countryDto = new CountryDto
            {
                Id = 1,
                Name = "Test Country",
                ImageUrl = "http://example.com/image.jpg"
            };
            _mapperMock.Setup(m => m.Map<Country>(It.IsAny<SetCountryDto>())).Returns(country);
            _fileServiceMock.Setup(f => f.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>())).ReturnsAsync("http://example.com/image.jpg");
            _unitOfWorkMock.Setup(u => u.Repository<Country, int>().AddAsync(It.IsAny<Country>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<CountryDto>(It.IsAny<Country>())).Returns(countryDto);
            // Act
            var result = await _countryService.AddAsync(setCountryDto);
            // Assert
            
            result.Data.Should().NotBeNull();
            result.Data.Should().BeEquivalentTo(countryDto);
            result.StatusCode.Should().Be(201);
            result.Message.Should().Be("Country added successfully.");

        }




        #endregion

    }
}