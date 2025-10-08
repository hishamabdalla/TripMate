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
        [Fact]
        public async Task AddHotel_AddHotelDtoIsNull_ThrowBadRequestException()
        {
            //Arrange
            AddHotelDto addHotel = null;
            //Act

            //Func<Task> action = () => _hotelService.AddHotelAsync(addHotel);
            //var exception = await Record.ExceptionAsync(action);

            var exception = await Record.ExceptionAsync(() => _hotelService.AddHotelAsync(addHotel));
            //Assert
            Assert.NotNull(exception);
            Assert.IsType<BadRequestException>(exception);
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
                Stars=4,
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
    }
}
