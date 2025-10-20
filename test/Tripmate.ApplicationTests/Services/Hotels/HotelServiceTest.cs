using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Abstractions.Hotel;
using Tripmate.Application.Services.Hotels;
using Tripmate.Application.Services.Hotels.DTOS;
using Tripmate.Application.Services.Image;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities.Models;
using Tripmate.Domain.Exceptions;
using Tripmate.Domain.Interfaces;
using Tripmate.Domain.Interfaces.Repositories.Intefaces;
using Tripmate.Domain.Specification.Hotels;
using Xunit;
using Assert = Xunit.Assert;

namespace Tripmate.ApplicationTests.Services.Hotels
{
    public class HotelServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<HotelServices>> _loggerMock;
        private readonly HotelServices _hotelService;

        public HotelServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _fileServiceMock = new Mock<IFileService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<HotelServices>>();

            _hotelService = new HotelServices(
               _mapperMock.Object,
               _unitOfWorkMock.Object,
               _loggerMock.Object,
               _fileServiceMock.Object
            );
        }
        #region AddHotelTest

        [Fact]
        public async Task AddHotel_AddHotelDtoIsNull_ThrowBadRequestException()
        {
           
            // Arrange
            AddHotelDto addHotelDto = null;
            // Act
            var exception = await Record.ExceptionAsync(() => _hotelService.AddHotelAsync(addHotelDto));
            // Assert
            Assert.IsType<BadRequestException>(exception);
            Assert.NotNull(exception);
            Assert.Equal("Invalid hotel data provided", exception.Message);


        }
        [Fact]
        public async Task AddHotel_ValidHotel_ReturnApiResponse()
        {
            //Mock iFormFile
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("text.jpg");
            fileMock.Setup(f => f.Length).Returns(1024);
            var formFile = fileMock.Object;
            //Arrange
            var addHotelDto = new AddHotelDto
            {
                Name="masa",
                Stars=4,
                PricePerNight="1400",
                RegionId=2,
                ImageUrl= formFile
            };
            var mappedHotel = new Hotel
            {
                Id=1,
                Name="masa",
                PricePerNight="1400",
                RegionId=2,
                ImageUrl = "path/to/image.jpg"
            };
            var readHotelDto = new ReadHotelDto
            {
                Name = "masa",
                Stars = 4,
                PricePerNight = "1400",
                RegionId = 2,
                ImageUrl = "path/to/image.jpg"
            };
               //Mock UploadImage
            _fileServiceMock.Setup(f => f.UploadImageAsync(formFile, "Hotels"))
                .ReturnsAsync("path/to/image.jpg");
               //Mock mapping
            _mapperMock.Setup(m => m.Map<Hotel>(addHotelDto)).Returns(mappedHotel);
            _mapperMock.Setup(m => m.Map<ReadHotelDto>(mappedHotel)).Returns(readHotelDto);
               //Mock Repo Add
            _unitOfWorkMock.Setup(u => u.Repository<Hotel, int>().AddAsync(mappedHotel))
                .Returns(Task.CompletedTask);
               //Mock SaveChangs
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);
            //Act
            var result = await _hotelService.AddHotelAsync(addHotelDto);
            //Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("Hotel added successfully.", result.Message);
            Assert.NotNull(result.Data);
            Assert.Equal("masa", result.Data.Name);
            Assert.Equal("path/to/image.jpg", result.Data.ImageUrl);
        }
        #endregion
        #region GetHotelTest

        [Fact]
        public async Task GetHotel_PageNumberEqualZerro_throwBadRequestException()
        {
            //Arrange
            HotelsParameters parameters = new HotelsParameters
            {
                PageNumber =0,
                PageSize=5
            };
            // Act
            var exception = await Record.ExceptionAsync(() => _hotelService.GetHotelsAsync(parameters));
            //Assert
            Assert.IsType<BadRequestException>(exception);
            Assert.NotNull(exception);
            Assert.Equal("PageNumber must be greater than 0.", exception.Message);
        }
        [Fact]
        public async Task GetHotel_PageSizeEqualZerro_throwBadRequestException()
        {
            //arrange
            HotelsParameters parameters = new HotelsParameters
            {
                PageNumber =5,
                PageSize=0
            };
            // Act
            var exception = await Record.ExceptionAsync(() => _hotelService.GetHotelsAsync(parameters));
            //Assert
            Assert.IsType<BadRequestException>(exception);
            Assert.NotNull(exception);
            Assert.Equal("PageSize must be greater than 0.", exception.Message);
        }
        [Fact]
        public async Task GetHotel_ValidParameters_ReturnsPaginationResponse()
        {
            //arrange
            var parameter = new HotelsParameters
            {
                PageNumber = 1,
                PageSize = 10
            };
            var hotelsList = new List<Hotel>
            {
                new Hotel { Id = 1, Name = "Hotel One", PricePerNight = "1000" },
                new Hotel { Id = 2, Name = "Hotel Two", PricePerNight = "1200" }
            };
            var mappedHotels = new List<ReadHotelDto>
            {
                new ReadHotelDto {Name = "Hotel One", PricePerNight = "1000" },
                new ReadHotelDto {Name = "Hotel Two", PricePerNight = "1200" }
            };
            _unitOfWorkMock.Setup(u => u.Repository<Hotel, int>().GetAllWithSpecAsync(It.IsAny<HotelsSpecification>()))
                .ReturnsAsync(hotelsList);
            _unitOfWorkMock.Setup(u => u.Repository<Hotel, int>().CountAsync(It.IsAny<HotelForCountingSpecification>()))
               .ReturnsAsync(2);
            _mapperMock.Setup(m => m.Map<IEnumerable<ReadHotelDto>>(hotelsList))
                .Returns(mappedHotels);
            //act
            var result = await _hotelService.GetHotelsAsync(parameter);
            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(10, result.PageSize);
            Assert.NotEmpty(result.Data);
            Assert.Equal("Hotel One", result.Data.First().Name);
        }
        [Fact]
        public async Task GetHotel_NoHotelsFound_throwNotFoundException()
        {
            var parameters = new HotelsParameters { PageNumber=1, PageSize=10 };
            _unitOfWorkMock.Setup(u => u.Repository<Hotel, int>().GetAllWithSpecAsync(It.IsAny<HotelsSpecification>()))
                .ReturnsAsync(new List<Hotel>());
            _unitOfWorkMock.Setup(u => u.Repository<Hotel, int>().CountAsync(It.IsAny<HotelForCountingSpecification>()))
               .ReturnsAsync(0);
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _hotelService.GetHotelsAsync(parameters));

        }
        #endregion
        [Fact]
        public async Task DeleteHotel_HotelExists_DeletesSuccessfully()
        {
            //Arrange
            int hotelId = 1;
            var hotel = new Hotel { Id=hotelId, ImageUrl="image.jpg"};
            var hotelRepoMock = new Mock<IGenericRepository<Hotel, int>>();
            hotelRepoMock.Setup(r => r.GetByIdAsync(hotelId)).ReturnsAsync(hotel);

            _unitOfWorkMock.Setup(u => u.Repository<Hotel, int>()).Returns(hotelRepoMock.Object);
            //Act
            var result = await _hotelService.DeleteHotel(hotelId);
            //Assert
            _fileServiceMock.Verify(f => f.DeleteImage("image.jpg", "Hotels"), Times.Once);
            hotelRepoMock.Verify(r => r.Delete(hotel), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("Hotel deleted successfully.", result.Message);

        }

        //public async Task<ApiResponse<bool>> DeleteHotel(int id)
        //{
        //    _logger.LogInformation("Deleting hotel with ID: {HotelId}", id);

        //    var hotel = await _unitOfWork.Repository<Hotel, int>().GetByIdAsync(id);
        //    if (hotel == null)
        //    {
        //        _logger.LogWarning("Hotel not found for deletion with ID: {HotelId}", id);
        //        throw new NotFoundException($"Hotel with ID {id} not found.");
        //    }

        //    if (!string.IsNullOrEmpty(hotel.ImageUrl))
        //    {
        //        _fileService.DeleteImage(hotel.ImageUrl, "Hotels");
        //        _logger.LogDebug("Deleted hotel image: {ImagePath}", hotel.ImageUrl);
        //    }

        //   

        //   
        //}

    }
}
