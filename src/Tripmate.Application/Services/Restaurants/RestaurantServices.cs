using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Abstractions.Restaurant;
using Tripmate.Application.Services.Attractions;
using Tripmate.Application.Services.Attractions.DTOs;
using Tripmate.Application.Services.Image;
using Tripmate.Application.Services.Restaurants.DTOS;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities.Models;
using Tripmate.Domain.Exceptions;
using Tripmate.Domain.Interfaces;
using Tripmate.Domain.Specification.Attractions;
using Tripmate.Domain.Specification.Restaurants;

namespace Tripmate.Application.Services.Restaurants
{
    public class RestaurantServices : IRestaurantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantServices> _logger;
        private readonly IFileService _fileService;

        public RestaurantServices(IUnitOfWork unitOfWork,IMapper mapper, ILogger<RestaurantServices> logger, IFileService fileService)
        {
            _unitOfWork=unitOfWork;
            _mapper=mapper;
            _logger=logger;
            _fileService=fileService;
        }
        public async Task<PaginationResponse<IEnumerable<ReadRestaurantDto>>> GetRestaurantsAsync(RestaurantsParameters parameters)
        {
            if (parameters.PageNumber <= 0)
                throw new BadRequestException("PageNumber must be greater than 0.");

            if (parameters.PageSize <= 0)
                throw new BadRequestException("PageSize must be greater than 0.");

            var dataSpec = new RestaurantSpecification(parameters);
            var countSpec = new RestaurantForCountingSpecification(parameters);
            var Restaurants = await _unitOfWork.Repository<Restaurant, int>().GetAllWithSpecAsync(dataSpec);
            var totalCount = await _unitOfWork.Repository<Restaurant, int>().CountAsync(countSpec);

            if (Restaurants == null || !Restaurants.Any())
            {
                _logger.LogWarning("No Restaurants found matching the provided criteria.");
                throw new NotFoundException("No Restaurants found matching the provided criteria.");
            }
            var RestaurantDtos = _mapper.Map<IEnumerable<ReadRestaurantDto>>(Restaurants);

            return new PaginationResponse<IEnumerable<ReadRestaurantDto>>(RestaurantDtos, totalCount, parameters.PageNumber,
                parameters.PageSize);
        }
        public async Task<ApiResponse<ReadRestaurantDto>> GetRestaurantByIdAsync(int id)
        {
            var restaurantSpecification = new RestaurantSpecification(id);
            var restaurants = await _unitOfWork.Repository<Restaurant, int>().GetByIdWithSpecAsync(restaurantSpecification);
            if(restaurants is null)
            {
                _logger.LogError($"Restaurant with ID {id} not found.");
                throw new NotFoundException($"Restaurant with ID {id} not found.");
            }
            var restaurantDto = _mapper.Map<ReadRestaurantDto>(restaurants);
            return new ApiResponse<ReadRestaurantDto>(restaurantDto)
            {
                Success = true,
                StatusCode = 200,
                Message = "Restaurant retrieved successfully."
            };
        }
        public async Task<ApiResponse<IEnumerable<ReadRestaurantDto>>> GetRestaurantByRegionIdAsync(int regionId)
        {
            var restaurantSpecification = new RestaurantSpecification(regionId, true);
            var restaurants = await _unitOfWork.Repository<Restaurant, int>().GetAllWithSpecAsync(restaurantSpecification);
            if (!restaurants.Any())
            {
                _logger.LogError($"No restaurants found in region with ID {regionId}.");
                throw new NotFoundException($"No restaurants found in region with ID {regionId}.");
            }
            var restaurantDtos = _mapper.Map<IEnumerable<ReadRestaurantDto>>(restaurants);
            return new ApiResponse<IEnumerable<ReadRestaurantDto>>(restaurantDtos)
            {
                Success = true,
                StatusCode = 200,
                Message = "Restaurants retrieved successfully."
            };
        }
        public async Task<ApiResponse<ReadRestaurantDto>> AddRestaurantAsync(AddRestaurantDto addRestaurantDto)
        {
            if (addRestaurantDto is null)
            {
                _logger.LogError("Invalid Restaurant data provided");
                throw new BadRequestException("Invalid Restaurant data provided");
            }
            
            _logger.LogInformation("Adding new restaurant: {RestaurantName} in region: {RegionId}", 
                addRestaurantDto.Name, addRestaurantDto.RegionId);
            
            string imagePath = null;
            if (addRestaurantDto.ImageUrl!=null&& addRestaurantDto.ImageUrl.Length>0)
            {
                _logger.LogDebug("Uploading image for restaurant: {RestaurantName}", addRestaurantDto.Name);
                imagePath= await _fileService.UploadImageAsync(addRestaurantDto.ImageUrl, "Restaurants");
                _logger.LogDebug("Image uploaded successfully: {ImagePath}", imagePath);
            }
            
            var restaurant = _mapper.Map<Restaurant>(addRestaurantDto);
            restaurant.ImageUrl=imagePath;
            
            await _unitOfWork.Repository<Restaurant, int>().AddAsync(restaurant);
            await _unitOfWork.SaveChangesAsync();
            
            var restaurantDto = _mapper.Map<ReadRestaurantDto>(restaurant);
            _logger.LogInformation("Successfully added restaurant: {RestaurantName} with ID: {RestaurantId}", 
                addRestaurantDto.Name, restaurant.Id);
            
            return new ApiResponse<ReadRestaurantDto>(restaurantDto)
            {
                Success = true,
                StatusCode = 200,
                Message = "Restaurant added successfully."
            };
        }

        public async Task<ApiResponse<ReadRestaurantDto>> UpdateRestaurantAsync(UpdateRestaurantDto updateRestaurantDto)
        {
            if (updateRestaurantDto is null)
            {
                _logger.LogError("Invalid Restaurant data provided");
                throw new BadRequestException("Invalid Restaurant data provided");
            }
            
            _logger.LogInformation("Updating restaurant with ID: {RestaurantId}", updateRestaurantDto.Id);
            
            var existingRestaurant = await _unitOfWork.Repository<Restaurant, int>().GetByIdAsync(updateRestaurantDto.Id);
            if (existingRestaurant is null)
            {
                _logger.LogWarning("Restaurant not found for update with ID: {RestaurantId}", updateRestaurantDto.Id);
                throw new NotFoundException("Restaurant not found.");
            }
            
            string imagePath = null;
            if (updateRestaurantDto.ImageUrl != null && updateRestaurantDto.ImageUrl.Length > 0)
            {
                _logger.LogDebug("Updating image for restaurant ID: {RestaurantId}", updateRestaurantDto.Id);
                
                if (!string.IsNullOrEmpty(existingRestaurant.ImageUrl))
                {
                    _fileService.DeleteImage(existingRestaurant.ImageUrl, "Restaurants");
                    _logger.LogDebug("Deleted old image: {OldImagePath}", existingRestaurant.ImageUrl);
                }
                
                imagePath = await _fileService.UploadImageAsync(updateRestaurantDto.ImageUrl, "Restaurants");
                _logger.LogDebug("New image uploaded: {ImagePath}", imagePath);
            }
            
            _mapper.Map(updateRestaurantDto, existingRestaurant);
            if (!string.IsNullOrEmpty(imagePath))
            {
                existingRestaurant.ImageUrl = imagePath;
            }
            
            await _unitOfWork.SaveChangesAsync();
            
            var restaurantDto = _mapper.Map<ReadRestaurantDto>(existingRestaurant);
            _logger.LogInformation("Successfully updated restaurant with ID: {RestaurantId}", updateRestaurantDto.Id);
            
            return new ApiResponse<ReadRestaurantDto>(restaurantDto)
            {
                Success = true,
                StatusCode = 200,
                Message = "Restaurant updated successfully."
            };
        }
        public async Task<ApiResponse<bool>> DeleteRestaurant(int id)
        {
            var restaurant = await _unitOfWork.Repository<Restaurant, int>().GetByIdAsync(id);
            if (restaurant == null)
            {
                _logger.LogWarning($"Restaurant with ID {id} not found for deletion.");
                throw new NotFoundException($"Restaurant with ID {id} not found.");
            }
            if (!string.IsNullOrEmpty(restaurant.ImageUrl))
            {
                _fileService.DeleteImage(restaurant.ImageUrl, "Restaurants");
            }
            _unitOfWork.Repository<Restaurant, int>().Delete(restaurant);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"Restaurant with ID {id} deleted successfully.");
            return new ApiResponse<bool>(true)
            {
                Success = true,
                StatusCode = 200, 
                Message = "Restaurant deleted successfully."
            };
        }
    }
}
