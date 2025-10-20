using Xunit;
using Tripmate.Application.Services.Attractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Tripmate.Domain.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Tripmate.Application.Services.Image;
using Tripmate.Domain.Specification.Attractions;
using Tripmate.Domain.Entities.Models;
using Tripmate.Application.Services.Attractions.DTOs;
using FluentAssertions;
using Tripmate.Domain.Exceptions;
using Tripmate.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Tripmate.Application.Services.Attractions.Tests
{
    public class AttractionServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<AttractionService>> _mockLogger;
        private readonly Mock<IFileService> _mockFileService;
        private readonly AttractionService _attractionService;

        public AttractionServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<AttractionService>>();
            _mockFileService = new Mock<IFileService>();
            _attractionService = new AttractionService(_mockUnitOfWork.Object,
                _mockMapper.Object,
                _mockLogger.Object,
                _mockFileService.Object);
        }

        // Add test methods here

        #region GetAttractionsAsync Tests
        [Fact]
        public async Task GetAttractionsAsync_ValidParameters_ReturnsPaginationResponse()
        {
            // Arrange
            var parameters = new AttractionParameter
            {
                PageNumber = 1,
                PageSize = 10,
                Search = "Attraction"


            };
            var attractions = new List<Attraction>
            {
                new Attraction { Id = 1, Name = "Attraction 1" },
                new Attraction { Id = 2, Name = "Attraction 2" }
            };
            _mockUnitOfWork.Setup(u => u.Repository<Attraction, int>().GetAllWithSpecAsync(It.IsAny<AttractionSpecification>()))
                .ReturnsAsync(attractions);
            _mockUnitOfWork.Setup(u => u.Repository<Attraction, int>().CountAsync(It.IsAny<AttractionsForCountingSpecification>()))
                .ReturnsAsync(attractions.Count);
            var attractionDtos = new List<AttractionDto>
            {
                new AttractionDto { Id = 1, Name = "Attraction 1" },
                new AttractionDto { Id = 2, Name = "Attraction 2" }
            };
            _mockMapper.Setup(m => m.Map<IEnumerable<AttractionDto>>(It.IsAny<IEnumerable<Attraction>>()))
                .Returns(attractionDtos);
            // Act
            var result = await _attractionService.GetAttractionsAsync(parameters);
            // Assert
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
            result.PageNumber.Should().Be(1);
            result.PageSize.Should().Be(10);


        }

        [Theory]
        [InlineData(0, 10, "PageNumber must be greater than 0.")]
        [InlineData(1, 0, "PageSize must be greater than 0.")]
        public async Task GetAttractionsAsync_InvalidParameters_ThrowsBadRequestException(int pageNumber, int pageSize, string expectedMessage)
        {
            // Arrange
            var parameters = new AttractionParameter
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Search = "Attraction"
            };
            // Act
            Func<Task> act = async () => await _attractionService.GetAttractionsAsync(parameters);
            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task GetAttractionsAsync_NoAttractionsFound_ThrowsNotFoundException()
        {
            // Arrange
            var parameters = new AttractionParameter
            {
                PageNumber = 1,
                PageSize = 10,
                Search = "NonExistentAttraction"
            };
            _mockUnitOfWork.Setup(u => u.Repository<Attraction, int>().GetAllWithSpecAsync(It.IsAny<AttractionSpecification>()))
                .ReturnsAsync(new List<Attraction>());
            _mockUnitOfWork.Setup(u => u.Repository<Attraction, int>().CountAsync(It.IsAny<AttractionsForCountingSpecification>()))
                .ReturnsAsync(0);
            // Act
            Func<Task> act = async () => await _attractionService.GetAttractionsAsync(parameters);
            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("No Attractions found.");
        }
        #endregion

        #region GetAttractionByIdAsync Tests
        [Fact]
        public async Task GetAttractionByIdAsync_ValidId_ReturnAttraction()
        {
            // Arrange
            var attractionId = 1;
            var attraction = new Attraction { Id = attractionId, Name = "Attraction 1" };

            var attractionDto = new AttractionDto { Id = attractionId, Name = "Attraction 1" };


            _mockUnitOfWork.Setup(u => u.Repository<Attraction, int>().GetByIdWithSpecAsync(It.IsAny<AttractionSpecification>()))
                .ReturnsAsync(attraction);
            _mockMapper.Setup(m => m.Map<AttractionDto>(It.IsAny<Attraction>()))
               .Returns(attractionDto);
            // Act
            var result = await _attractionService.GetAttractionByIdAsync(attractionId);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(attractionId);
        }

        [Fact]
        public async Task GetAttractionByIdAsync_InvalidId_ThrowsNotFoundException()
        {
            // Arrange
            var attractionId = 999; // Non-existent ID

            Attraction attraction = null;
            _mockUnitOfWork.Setup(u => u.Repository<Attraction, int>().GetByIdWithSpecAsync(It.IsAny<AttractionSpecification>()))
                .ReturnsAsync(attraction);
            // Act
            Func<Task> act = async () => await _attractionService.GetAttractionByIdAsync(attractionId);
            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Attraction with ID {attractionId} not found.");
        }

        #endregion

        #region AddAsync Tests
        [Fact]
        public async Task AddAsync_ValidData_ReturnsAttractionDto()
        {

            // Arrange

            var moqFile = new Mock<IFormFile>();
            moqFile.Setup(f => f.FileName).Returns("image.jpg");
            moqFile.Setup(f => f.Length).Returns(1024 * 1024); // 1 MB

            var region = new Region { Id = 1, Name = "Region 1" };

            var setAttractionDto = new SetAttractionDto
            {
                Name = "New Attraction",
                Description = "A new attraction",
                Type = AttractionType.Museum,
                ImageUrl = moqFile.Object, // Assuming no image for simplicity
                RegionId = region.Id
            };
            var attraction = new Attraction
            {
                Id = 1,
                Name = setAttractionDto.Name,
                Description = setAttractionDto.Description,
                Type = AttractionType.Museum,
                ImageUrl = "http://example.com/image.jpg"
            };
            var attractionDto = new AttractionDto
            {
                Id = 1,
                Name = setAttractionDto.Name,
                Description = setAttractionDto.Description,
                Type = setAttractionDto.Type.ToString(),
                ImageUrl = "http://example.com/image.jpg"
            };
            _mockMapper.Setup(m => m.Map<Attraction>(It.IsAny<SetAttractionDto>()))
                .Returns(attraction);
            _mockUnitOfWork.Setup(u => u.Repository<Attraction, int>().AddAsync(It.IsAny<Attraction>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<AttractionDto>(It.IsAny<Attraction>()))
                .Returns(attractionDto);
            _mockFileService.Setup(f => f.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
                .ReturnsAsync("http://example.com/image.jpg");

            _mockUnitOfWork.Setup(u => u.Repository<Region, int>().GetByIdAsync(setAttractionDto.RegionId))
                .ReturnsAsync(region);


            // Act
            var result = await _attractionService.AddAsync(setAttractionDto);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(1);
            result.Data.Name.Should().Be(setAttractionDto.Name);



        }

        [Fact]
        public async Task AddAsync_NullSetAttractionDto_ThrowsArgumentNullException()
        {
            // Arrange
            SetAttractionDto setAttractionDto = null;
            // Act
            Func<Task> act = async () => await _attractionService.AddAsync(setAttractionDto);
            // Assert
            await act.Should().ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task AddAsync_NullImageUrl_ThrowsBadRequestException()
        {
            // Arrange
            var setAttractionDto = new SetAttractionDto
            {
                Name = "New Attraction",
                Description = "A new attraction",
                Type = AttractionType.Safari,
                ImageUrl = null, // Null image
                RegionId = 1
            };
            // Act
            Func<Task> act = async () => await _attractionService.AddAsync(setAttractionDto);
            // Assert
            await act.Should().ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task AddAsync_InvalidRegionId_ThrowsNotFoundException()
        {
            // Arrange
            var moqFile = new Mock<IFormFile>();
            moqFile.Setup(f => f.FileName).Returns("image.jpg");
            moqFile.Setup(f => f.Length).Returns(1024 * 1024); // 1 MB
            var setAttractionDto = new SetAttractionDto
            {
                Name = "New Attraction",
                Description = "A new attraction",
                Type = AttractionType.Museum ,   
                ImageUrl = moqFile.Object,
                RegionId = 999 // Non-existent region
            };
            _mockUnitOfWork.Setup(u => u.Repository<Region, int>().GetByIdAsync(setAttractionDto.RegionId))
                .ReturnsAsync((Region)null); // Region not found
            // Act
            Func<Task> act = async () => await _attractionService.AddAsync(setAttractionDto);
            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        #endregion

        #region UpdateAsync Tests
        [Fact]
        public async Task UpdateAsync_ValidData_ReturnsUpdatedAttractionDto()
        {
            // Arrange
            var attractionId = 1;
            var moqFile = new Mock<IFormFile>();
            moqFile.Setup(f => f.FileName).Returns("updated_image.jpg");
            moqFile.Setup(f => f.Length).Returns(1024 * 1024); // 1 MB
            var existingAttraction = new Attraction
            {
                Id = attractionId,
                Name = "Old Attraction",
                Description = "An old attraction",
                Type = AttractionType.Park,
                ImageUrl = "http://example.com/old_image.jpg",
                RegionId = 1
            };
            var region = new Region { Id = 2, Name = "Region 2" };
            var setAttractionDto = new SetAttractionDto
            {
                Name = "Updated Attraction",
                Description = "An updated attraction",
                Type = AttractionType.Museum,
                ImageUrl = moqFile.Object,
                RegionId = region.Id
            };
            var updatedAttraction = new Attraction
            {
                Id = attractionId,
                Name = setAttractionDto.Name,
                Description = setAttractionDto.Description,
                Type = AttractionType.Museum,
                ImageUrl = "http://example.com/updated_image.jpg",
                RegionId = region.Id
            };
            var updatedAttractionDto = new AttractionDto
            {
                Id = attractionId,
                Name = setAttractionDto.Name,
                Description = setAttractionDto.Description,
                Type = setAttractionDto.Type.ToString(),
                ImageUrl = "http://example.com/updated_image.jpg"
            };
            _mockUnitOfWork.Setup(u => u.Repository<Attraction, int>().GetByIdAsync(attractionId))
                .ReturnsAsync(existingAttraction);
            _mockUnitOfWork.Setup(u => u.Repository<Region, int>().GetByIdAsync(setAttractionDto.RegionId))
                .ReturnsAsync(region);
            _mockMapper.Setup(m => m.Map(setAttractionDto, existingAttraction))
                .Returns(updatedAttraction);
            _mockUnitOfWork.Setup(u => u.Repository<Attraction, int>().Update(It.IsAny<Attraction>()));
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<AttractionDto>(It.IsAny<Attraction>()))
                .Returns(updatedAttractionDto);
            _mockFileService.Setup(f => f.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
                .ReturnsAsync("http://example.com/updated_image.jpg");
            // Act
            var result = await _attractionService.UpdateAsync(attractionId, setAttractionDto);
            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(attractionId);
            result.Data.Name.Should().Be(setAttractionDto.Name);
        }

        [Fact]
        public async Task UpdateAsync_NonExistentAttraction_ThrowsNotFoundException()
        {
            // Arrange
            var attractionId = 999; // Non-existent ID
            var setAttractionDto = new SetAttractionDto
            {
                Name = "Updated Attraction",
                Description = "An updated attraction",
                Type = AttractionType.Museum,
                ImageUrl = null,
                RegionId = 1
            };
            _mockUnitOfWork.Setup(u => u.Repository<Attraction, int>().GetByIdAsync(attractionId))
                .ReturnsAsync((Attraction)null); // Attraction not found
            // Act
            Func<Task> act = async () => await _attractionService.UpdateAsync(attractionId, setAttractionDto);
            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Attraction with ID {attractionId} not found.");
        }

        [Fact]
        public async Task UpdateAsync_NullSetAttractionDto_ThrowsArgumentNullException()
        {
            // Arrange
            var attractionId = 1;
            SetAttractionDto setAttractionDto = null;
            // Act
            Func<Task> act = async () => await _attractionService.UpdateAsync(attractionId, setAttractionDto);
            // Assert
            await act.Should().ThrowAsync<BadRequestException>();
        }




        [Fact]
        public async Task UpdateAsync_InvalidRegionId_ThrowsNotFoundException()
        {
            // Arrange
            var attractionId = 1;
            var existingAttraction = new Attraction
            {
                Id = attractionId,
                Name = "Old Attraction",
                Description = "An old attraction",
                Type = AttractionType.Park,
                ImageUrl = "http://example.com/old_image.jpg",
                RegionId = 1
            };
            var setAttractionDto = new SetAttractionDto
            {
                Name = "Updated Attraction",
                Description = "An updated attraction",
                Type = AttractionType.Museum,
                ImageUrl = null,
                RegionId = 999 // Non-existent region
            };
            _mockUnitOfWork.Setup(u => u.Repository<Attraction, int>().GetByIdAsync(attractionId))
                .ReturnsAsync(existingAttraction);
            _mockUnitOfWork.Setup(u => u.Repository<Region, int>().GetByIdAsync(setAttractionDto.RegionId))
                .ReturnsAsync((Region)null); // Region not found
            // Act
            Func<Task> act = async () => await _attractionService.UpdateAsync(attractionId, setAttractionDto);
            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ValidId_DeletesAttraction()
        {
            // Arrange
            var attractionId = 1;
            var existingAttraction = new Attraction
            {
                Id = attractionId,
                Name = "Attraction to be deleted"
            };
            _mockUnitOfWork.Setup(u => u.Repository<Attraction, int>().GetByIdAsync(attractionId))
                .ReturnsAsync(existingAttraction);
            _mockUnitOfWork.Setup(u => u.Repository<Attraction, int>().Delete(It.IsAny<Attraction>()));
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);
            // Act
            var result = await _attractionService.DeleteAsync(attractionId);
            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Attraction deleted successfully.");
        }

        [Fact]
        public async Task DeleteAsync_NonExistentId_ThrowsNotFoundException()
        {
            // Arrange
            var attractionId = 999; // Non-existent ID
            _mockUnitOfWork.Setup(u => u.Repository<Attraction, int>().GetByIdAsync(attractionId))
                .ReturnsAsync((Attraction)null); // Attraction not found
            // Act
            Func<Task> act = async () => await _attractionService.DeleteAsync(attractionId);
            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        #endregion



    }
    }
    