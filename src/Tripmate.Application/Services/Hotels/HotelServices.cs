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

            var hotelDto = _mapper.Map<ReadHotelDto>(hotel);
            _logger.LogInformation("Successfully retrieved hotel with ID: {HotelId}", id);
            return new ApiResponse<ReadHotelDto>(hotelDto)
            {
                Success = true,
                StatusCode = 200,
                Message = "Hotel retrieved successfully."
            };
        }

        public async Task<ApiResponse<IEnumerable<ReadHotelDto>>> GetHotelsByRegionIdAsync(int regionId)
        {
            _logger.LogInformation("Getting hotels by region ID: {RegionId}", regionId);
            
            var hotelSpecification = new HotelsSpecification(regionId, true);
            var hotels = await _unitOfWork.Repository<Hotel, int>().GetAllWithSpecAsync(hotelSpecification);
            
            if (!hotels.Any())
            {
                _logger.LogWarning("No hotels found for region ID: {RegionId}", regionId);
                throw new NotFoundException($"No hotels found in region with ID {regionId}.");
            }
            
            var hotelDtos = _mapper.Map<IEnumerable<ReadHotelDto>>(hotels);
            _logger.LogInformation("Successfully retrieved {Count} hotels for region ID: {RegionId}", hotels.Count(), regionId);
            
            return new ApiResponse<IEnumerable<ReadHotelDto>>(hotelDtos)
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
                _logger.LogError("Invalid hotel data provided for addition");
                throw new BadRequestException("Invalid hotel data provided");
            }
            _logger.LogInformation("Adding new hotel: {HotelName} in region: {RegionId}", addHotelDto.Name, addHotelDto.RegionId);

            string imagePath = null;
            if (addHotelDto.ImageUrl != null && addHotelDto.ImageUrl.Length > 0)
            {
                _logger.LogDebug("Uploading image for hotel: {HotelName}", addHotelDto.Name);
                imagePath = await _fileService.UploadImageAsync(addHotelDto.ImageUrl, "Hotels");
                _logger.LogDebug("Image uploaded successfully: {ImagePath}", imagePath);
            }
            
            var hotel = _mapper.Map<Hotel>(addHotelDto);
            hotel.ImageUrl = imagePath;
            
            await _unitOfWork.Repository<Hotel, int>().AddAsync(hotel);
            await _unitOfWork.SaveChangesAsync();
            
            var hotelDto = _mapper.Map<ReadHotelDto>(hotel);
            _logger.LogInformation("Successfully added hotel: {HotelName} with ID: {HotelId}", addHotelDto.Name, hotel.Id);
            
            return new ApiResponse<ReadHotelDto>(hotelDto)
            {
                Success = true,
                StatusCode = 200,
                Message = "Hotel added successfully."
            };
        }

        public async Task<ApiResponse<ReadHotelDto>> UpdateHotelAsync(UpdateHotelDto updateHotelDto)
        {
            _logger.LogInformation("Updating hotel with ID: {HotelId}", updateHotelDto.Id);
            
            if (updateHotelDto is null)
            {
                _logger.LogError("Invalid hotel data provided for update");
                throw new BadRequestException("Invalid hotel data provided");
            }
            
            var existingHotel = await _unitOfWork.Repository<Hotel, int>().GetByIdAsync(updateHotelDto.Id);
            if (existingHotel is null)
            {
                _logger.LogWarning("Hotel not found for update with ID: {HotelId}", updateHotelDto.Id);
                throw new NotFoundException("Hotel not found.");
            }
            
            string imagePath = null;
            if (updateHotelDto.ImageUrl != null && updateHotelDto.ImageUrl.Length > 0)
            {
                _logger.LogDebug("Updating image for hotel ID: {HotelId}", updateHotelDto.Id);
                
                if (!string.IsNullOrEmpty(existingHotel.ImageUrl))
                {
                    _fileService.DeleteImage(existingHotel.ImageUrl, "Hotels");
                    _logger.LogDebug("Deleted old image: {OldImagePath}", existingHotel.ImageUrl);
                }
                
                imagePath = await _fileService.UploadImageAsync(updateHotelDto.ImageUrl, "Hotels");
                _logger.LogDebug("New image uploaded: {ImagePath}", imagePath);
            }
            
            _mapper.Map(updateHotelDto, existingHotel);
            if (!string.IsNullOrEmpty(imagePath))
            {
                existingHotel.ImageUrl = imagePath;
            }
            
            await _unitOfWork.SaveChangesAsync();
            
            var hotelDto = _mapper.Map<ReadHotelDto>(existingHotel);
            _logger.LogInformation("Successfully updated hotel with ID: {HotelId}", updateHotelDto.Id);
            
            return new ApiResponse<ReadHotelDto>(hotelDto)
            {
                Success = true,
                StatusCode = 200,
                Message = "Hotel updated successfully."
            };
        }

        public async Task<ApiResponse<bool>> DeleteHotel(int id)
        {
            _logger.LogInformation("Deleting hotel with ID: {HotelId}", id);
            
            var hotel = await _unitOfWork.Repository<Hotel, int>().GetByIdAsync(id);
            if (hotel == null)
            {
                _logger.LogWarning("Hotel not found for deletion with ID: {HotelId}", id);
                throw new NotFoundException($"Hotel with ID {id} not found.");
            }
            
            if (!string.IsNullOrEmpty(hotel.ImageUrl))
            {
                _fileService.DeleteImage(hotel.ImageUrl, "Hotels");
                _logger.LogDebug("Deleted hotel image: {ImagePath}", hotel.ImageUrl);
            }
            
            _unitOfWork.Repository<Hotel, int>().Delete(hotel);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Successfully deleted hotel with ID: {HotelId}", id);
            
            return new ApiResponse<bool>(true)
            {
                Success = true,
                StatusCode = 200,
                Message = "Hotel deleted successfully."
            };
        }
    }
}
