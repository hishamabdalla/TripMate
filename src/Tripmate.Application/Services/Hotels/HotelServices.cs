using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Abstractions.Hotel;
using Tripmate.Application.Services.Countries.DTOs;
using Tripmate.Application.Services.Hotels.DTOS;
using Tripmate.Application.Services.Image;
using Tripmate.Application.Services.Restaurants.DTOS;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities.Models;
using Tripmate.Domain.Exceptions;
using Tripmate.Domain.Interfaces;
using Tripmate.Domain.Specification.Hotels;
using Tripmate.Domain.Specification.Restaurants;
using static StackExchange.Redis.Role;

namespace Tripmate.Application.Services.Hotels
{
    public class HotelServices : IHotelServices
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HotelServices> _logger;
        private readonly IFileService _fileService;
        public HotelServices(IMapper mapper,IUnitOfWork unitOfWork,ILogger<HotelServices> logger,IFileService fileService)
        {
            _mapper=mapper;
            _unitOfWork=unitOfWork;
            _logger=logger;
            _fileService=fileService;
        }
        public async Task<PaginationResponse<IEnumerable<ReadHotelDto>>> GetHotelsAsync(HotelsParameters parameters)
        {
            if (parameters.PageNumber <= 0)
                throw new BadRequestException("PageNumber must be greater than 0.");

            if (parameters.PageSize <= 0)
                throw new BadRequestException("PageSize must be greater than 0.");

            var dataSpec = new HotelsSpecification(parameters);
            var countSpec = new HotelForCountingSpecification(parameters);
            var hotels = await _unitOfWork.Repository<Hotel, int>().GetAllWithSpecAsync(dataSpec);
            var totalCount = await _unitOfWork.Repository<Hotel, int>().CountAsync(countSpec);

            if (hotels == null || !hotels.Any())
            {
                _logger.LogWarning("No hotels found matching the provided criteria.");
                throw new NotFoundException("No hotels found matching the provided criteria.");
            }
            var hotelsDto = _mapper.Map<IEnumerable<ReadHotelDto>>(hotels);

            return new PaginationResponse<IEnumerable<ReadHotelDto>>(hotelsDto, totalCount, parameters.PageNumber,
                parameters.PageSize);
        }
        public async Task<ApiResponse<ReadHotelDto>> GetHotelByIdAsync(int id)
        {
            var hotelsSpecifications = new HotelsSpecification(id);
            var hotel = await _unitOfWork.Repository<Hotel, int>().GetByIdWithSpecAsync(hotelsSpecifications);
            if(hotel is null)
            {
                _logger.LogError($"Hotel with ID {id} not found.");
                throw new NotFoundException($"Hotel with ID {id} not found.");
            }
            var hotelDto=_mapper.Map<ReadHotelDto>(hotel);
            return new ApiResponse<ReadHotelDto>(hotelDto)
            {
                Success=true,
                StatusCode=200,
                Message="Hotel retrieved successfully"
            };
        }
        public async Task<ApiResponse<IEnumerable<ReadHotelDto>>> GetHotelsByRegionIdAsync(int regionId)
        {
            var hotelsSpecifications = new HotelsSpecification(regionId, true);
            var hotel = await _unitOfWork.Repository<Hotel, int>().GetAllWithSpecAsync(hotelsSpecifications);
            if (!hotel.Any())
            {
                _logger.LogError($"No hotels found in region with ID {regionId}.");
                throw new NotFoundException($"No hotels found in region with ID {regionId}.");
            }
            var hotelsDto = _mapper.Map<IEnumerable<ReadHotelDto>>(hotel);
            return new ApiResponse<IEnumerable<ReadHotelDto>>(hotelsDto)
            {
                Success = true,
                StatusCode = 200,
                Message = "Hotels retrieved successfully."
            };
        }
        public async Task<ApiResponse<ReadHotelDto>> AddHotelAsync(AddHotelDto addHotelDto)
        {
            if (addHotelDto is null)
            {
                _logger.LogError("Invalid hotel data provided");
                throw new BadRequestException("Invalid hotel data provided");
            }
            string imagePath = null;
            if (addHotelDto.ImageUrl!=null&& addHotelDto.ImageUrl.Length>0)
            {
                imagePath= await _fileService.UploadImageAsync(addHotelDto.ImageUrl, "Hotels");
            }
            var hotel = _mapper.Map<Hotel>(addHotelDto);
            hotel.ImageUrl=imagePath;
            await _unitOfWork.Repository<Hotel, int>().AddAsync(hotel);
            await _unitOfWork.SaveChangesAsync();
            var hotelDto = _mapper.Map<ReadHotelDto>(hotel);
            return new ApiResponse<ReadHotelDto>(hotelDto)
            {
                Success = true,
                StatusCode = 200,
                Message = "Hotel added successfully."
            };
            
          
        }
        public async Task<ApiResponse<ReadHotelDto>> UpdateHotelAsync(UpdateHotelDto updateHotelDto)
        {
            if(updateHotelDto is null)
            {
                _logger.LogError("Invalid hotel data provided");
                throw new BadRequestException("Invalid hotel data provided");
            }
            var existingHotel = await _unitOfWork.Repository<Hotel,int>().GetByIdAsync(updateHotelDto.Id);
            if(existingHotel is null)
            {
                _logger.LogWarning("Hotel not found for update.");
                throw new NotFoundException("Hotel not found for update.");
            }
            string imagePath = null;
            if (updateHotelDto.ImageUrl != null && updateHotelDto.ImageUrl.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingHotel.ImageUrl))
                {
                    _fileService.DeleteImage(existingHotel.ImageUrl, "Hotels");
                }
                imagePath = await _fileService.UploadImageAsync(updateHotelDto.ImageUrl, "Hotels");
            }
            _mapper.Map(updateHotelDto, existingHotel);
            if (!string.IsNullOrEmpty(imagePath))
            {
                existingHotel.ImageUrl = imagePath;
            }
           
             await _unitOfWork.SaveChangesAsync();
            var hotelDto = _mapper.Map<ReadHotelDto>(existingHotel);
            return new ApiResponse<ReadHotelDto>(hotelDto)
            {
                Success = true,
                StatusCode = 200,
                Message = "Hotel updated successfully."
            };

        }
        public async Task<ApiResponse<bool>> DeleteHotel(int id)
        {
            var existingHotel = await _unitOfWork.Repository<Hotel, int>().GetByIdAsync(id);
            if (existingHotel is null)
            {
                _logger.LogWarning("Hotel not found for update.");
                throw new NotFoundException("Hotel not found for update.");
            }
            if (!string.IsNullOrEmpty(existingHotel.ImageUrl))
            {
                _fileService.DeleteImage(existingHotel.ImageUrl, "Hotels");
            }
            _unitOfWork.Repository<Hotel, int>().Delete(existingHotel);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"Hotel with ID {id} deleted successfully.");
            return new ApiResponse<bool>(true)
            {
                Success = true,
                StatusCode = 200,
                Message = "Hotel deleted successfully."
            };
        }
    }
}
