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
using Tripmate.Domain.Exceptions;
using Tripmate.Domain.Interfaces;
using Assert= Xunit.Assert;
using static System.Net.WebRequestMethods;
using Xunit;
using Tripmate.Domain.Specification.Countries;

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

        #region AddAsync Test

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

        #region Invalid Test Cases 
        [Fact]
        public async Task AddAsync_NullCountry_ThrowsBadRequestException()
        {

            //Arrange
            SetCountryDto setCountryDto = null;

            //Act
            Func<Task> result = async () => await _countryService.AddAsync(setCountryDto);

            //Assert
            await result.Should().ThrowAsync<BadRequestException>().WithMessage("Country data cannot be null.");

        }

        [Fact]
        public async Task AddAsync_NullImageUrl_ThrowsBadRequestException()
        {
            //Arrange
            SetCountryDto setCountryDto = new()
            {
                Name = "Test",
                ImageUrl = null,
            };

            //Act
            Func<Task> result = async () => await _countryService.AddAsync(setCountryDto);

            //Assert
            await result.Should().ThrowAsync<BadRequestException>().WithMessage("ImageUrl is required.");

        }

        #endregion

        #endregion

        #region GetCountriesAsync Test

        #region Valid Test Cases

        [Fact]
        public async Task GetCountriesAsync_ValidParameters_ReturnPagedData()
        {
            //Arrange
            CountryParameters countryParameters = new()
            {
                PageNumber = 1,
                PageSize = 10,
                Search = "Country1"
            };
            var countries = new List<Country>
            {
                new Country { Id = 1, Name = "Country1", ImageUrl = "http://example.com/image1.jpg" },
                new Country { Id = 2, Name = "Country2", ImageUrl = "http://example.com/image2.jpg" },
            };

            var countryDtos = new List<CountryDto>
            {
                new CountryDto { Id = 1, Name = "Country1", ImageUrl = "http://example.com/image1.jpg" },
                new CountryDto { Id = 2, Name = "Country2", ImageUrl = "http://example.com/image2.jpg" },
            };

            _unitOfWorkMock.Setup(u => u.Repository<Country, int>().GetAllWithSpecAsync(It.IsAny<CountrySpecification>())).ReturnsAsync(countries);
            _unitOfWorkMock.Setup(u => u.Repository<Country, int>().CountAsync(It.IsAny<CountriesForCountingSpecification>())).ReturnsAsync(countries.Count);
            _mapperMock.Setup(m => m.Map<IEnumerable<CountryDto>>(It.IsAny<IEnumerable<Country>>())).Returns(countryDtos);

            //Act
            var result = await _countryService.GetCountriesAsync(countryParameters);

            //Assert
            result.Data.Should().NotBeNull();
            result.Data.Should().BeEquivalentTo(countryDtos);


        }

        #endregion

        #region Invalid Test Cases
        [Theory]
        [InlineData(0, 10, "PageNumber must be greater than 0.")]
        [InlineData(1, 0, "PageSize must be greater than 0.")]
        public async Task GetCountriesAsync_InvalidParameters_ThrowsBadRequestException(int pageNumber, int pageSize, string expectedMessage)
        {
            //Arrange
            CountryParameters countryParameters = new()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
            //Act
            Func<Task> result = async () => await _countryService.GetCountriesAsync(countryParameters);
            //Assert
            await result.Should().ThrowAsync<BadRequestException>().WithMessage(expectedMessage);
        }

        [Fact]
        public async Task GetCountriesAsync_NoCountriesFound_ThrowsNotFoundException()
        {
            //Arrange
            CountryParameters countryParameters = new()
            {
                PageNumber = 1,
                PageSize = 10,

            };
            var countries = new List<Country>();

            _unitOfWorkMock.Setup(u => u.Repository<Country, int>().GetAllWithSpecAsync(It.IsAny<CountrySpecification>())).ReturnsAsync(countries);
            _unitOfWorkMock.Setup(u => u.Repository<Country, int>().CountAsync(It.IsAny<CountriesForCountingSpecification>())).ReturnsAsync(0);
            //Act
            Func<Task> result = async () => await _countryService.GetCountriesAsync(countryParameters);
            //Assert
            await result.Should().ThrowAsync<NotFoundException>().WithMessage("No countries found matching the provided criteria.");
        }

        #endregion

        #endregion

        #region GetCountryByIdAsync Test

        #region Valid Test Cases
        [Fact]
        public async Task GetCountryByIdAsync_ValidId_ReturnsCountryDto()
        {
            //Arrange
            int countryId = 1;

            var country = new Country
            {
                Id = countryId,
                Name = "Country1",
                ImageUrl = "http://example.com/image1.jpg"
            };
            var countryDto = new CountryDto
            {
                Id = countryId,
                Name = "Country1",
                ImageUrl = "http://example.com/image1.jpg"
            };

            _unitOfWorkMock.Setup(u => u.Repository<Country, int>().GetByIdWithSpecAsync(It.IsAny<CountrySpecification>())).ReturnsAsync(country);
            _mapperMock.Setup(m => m.Map<CountryDto>(It.IsAny<Country>())).Returns(countryDto);

            //Act

            var result = await _countryService.GetCountryByIdAsync(countryId);

            //Assert
            result.Should().NotBeNull();
            result.Data.Should().BeEquivalentTo(countryDto);


        }

        #endregion

        #region Invalid Test Cases
        [Fact]
        public async Task GetCountryByIdAsync_NonExistentId_ThrowsNotFoundException()
        {
            //Arrange
            int countryId = 1;
            Country country = null;

            _unitOfWorkMock.Setup(u => u.Repository<Country, int>().GetByIdWithSpecAsync(It.IsAny<CountrySpecification>())).ReturnsAsync(country);
            //Act
            Func<Task> result = async () => await _countryService.GetCountryByIdAsync(countryId);
            //Assert
            await result.Should().ThrowAsync<NotFoundException>();
        }


        #endregion

        #endregion

        #region UpdateAsync Test

        #region Valid Test Cases
        [Fact]
        public async Task UpdateAsync_ValidCountry_ReturnsApiResponse()
        {
            // Arrange
            int countryId = 1;
            var moqFile = new Mock<IFormFile>();
            moqFile.Setup(f => f.FileName).Returns("image.jpg");
            moqFile.Setup(f => f.Length).Returns(1024);
            var setCountryDto = new SetCountryDto
            {
                Name = "Updated Country",
                ImageUrl = moqFile.Object // Assuming image upload is handled separately
            };
            var existingCountry = new Country
            {
                Id = countryId,
                Name = "Old Country",
                ImageUrl = "http://example.com/oldimage.jpg"
            };
            var updatedCountry = new Country
            {
                Id = countryId,
                Name = "Updated Country",
                ImageUrl = "http://example.com/newimage.jpg"
            };
            var countryDto = new CountryDto
            {
                Id = countryId,
                Name = "Updated Country",
                ImageUrl = "http://example.com/newimage.jpg"
            };
            _unitOfWorkMock.Setup(u => u.Repository<Country, int>().GetByIdAsync(countryId)).ReturnsAsync(existingCountry);
            _mapperMock.Setup(m => m.Map(setCountryDto, existingCountry)).Returns(updatedCountry);

            _fileServiceMock.Setup(f => f.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>())).ReturnsAsync("http://example.com/newimage.jpg");
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<CountryDto>(It.IsAny<Country>())).Returns(countryDto);

            // Act
            var result = await _countryService.Update(countryId, setCountryDto);

            // Assert
            result.Data.Should().NotBeNull();
            result.Data.Should().BeEquivalentTo(countryDto);



        }
        #endregion

        #region Invalid Test Cases
        [Fact]
        public async Task UpdateAsync_NullCountry_ThrowsBadRequestException()
        {
            //Arrange
            int countryId = 1;
            SetCountryDto setCountryDto = null;
            //Act
            Func<Task> result = async () => await _countryService.Update(countryId, setCountryDto);
            //Assert
            await result.Should().ThrowAsync<BadRequestException>().WithMessage("Country data cannot be null.");
        }

        [Fact]
        public async Task UpdateAsync_NonExistentId_ThrowsNotFoundException()
        {
            int countryId = 1;
            var setCountryDto = new SetCountryDto
            {
                Name = "Updated Country",
                ImageUrl = null // Assuming image upload is handled separately
            };
            Country country = null;
            _unitOfWorkMock.Setup(u => u.Repository<Country, int>().GetByIdAsync(countryId)).ReturnsAsync(country);

            //Act
            Func<Task> result = async () => await _countryService.Update(countryId, setCountryDto);

            //Assert

            await result.Should().ThrowAsync<NotFoundException>();

        }


        #endregion
        #endregion

        #region DeleteAsync Test

        #region Valid Test Cases


        [Fact]
        public async Task DeleteAsync_ValidId_ReturnsApiResponse()
        {
            //Arrange
            int countryId = 1;
            var existingCountry = new Country
            {
                Id = countryId,
                Name = "Country to be deleted",
                ImageUrl = "http://example.com/image.jpg"
            };
            _unitOfWorkMock.Setup(u => u.Repository<Country, int>().GetByIdAsync(countryId)).ReturnsAsync(existingCountry);
            _unitOfWorkMock.Setup(u => u.Repository<Country, int>().Delete(existingCountry));
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);
            //Act
            var result = await _countryService.Delete(countryId);
            //Assert
            result.Should().NotBeNull();
            result.Data.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Message.Should().Be("Country deleted successfully.");
        }


        #endregion

        #region Invalid Test Cases

        [Fact]
        public async Task DeleteAsync_NonExistentId_ThrowsNotFoundException()
        {
            //Arrange
            int countryId = 1;
            Country country = null;
            _unitOfWorkMock.Setup(u => u.Repository<Country, int>().GetByIdAsync(countryId)).ReturnsAsync(country);

            //Act
            Func<Task> result = async () => await _countryService.Delete(countryId);

            //Assert
            await result.Should().ThrowAsync<NotFoundException>();
        }

        #endregion
        #endregion


    }
}