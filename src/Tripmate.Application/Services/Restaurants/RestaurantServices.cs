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

        public RestaurantServices(IUnitOfWork unitOfWork,IMapper mapper, ILogger<RestaurantServices> logger)
        {
            _unitOfWork=unitOfWork;
            _mapper=mapper;
            _logger=logger;
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
        public async Task<ApiResponse<ReadRestaurantDto>> AddRestaurantAsync(AddRestaurantDto addRestaurantDto, string webRootPath)
        {
            if(addRestaurantDto is null)
            {
                _logger.LogError("Invalid Restaurant data provided");
                throw new BadRequestException("Invalid Restaurant data provided");
            }
            string imagePath = null;
            if (addRestaurantDto.ImageUrl!=null&& addRestaurantDto.ImageUrl.Length>0)
            {
                imagePath= await SaveImageAsync(addRestaurantDto.ImageUrl, webRootPath);
            }
            var restaurant = _mapper.Map<Restaurant>(addRestaurantDto);
            restaurant.ImageUrl=imagePath;
            await _unitOfWork.Repository<Restaurant, int>().AddAsync(restaurant);
            await _unitOfWork.SaveChangesAsync();
            var restaurantDto = _mapper.Map<ReadRestaurantDto>(restaurant);
            return new ApiResponse<ReadRestaurantDto>(restaurantDto)
            {
                Success = true,
                StatusCode = 200,
                Message = "Restaurant added successfully."
            };
        }
        public async Task<ApiResponse<ReadRestaurantDto>> UpdateRestaurantAsync(UpdateRestaurantDto updateRestaurantDto, string webRootPath)
        {
            if (updateRestaurantDto is null)
            {
                _logger.LogError("Invalid Restaurant data provided");
                throw new BadRequestException("Invalid Restaurant data provided");
            }
            var existingRestaurant = await _unitOfWork.Repository<Restaurant, int>().GetByIdAsync(updateRestaurantDto.Id);
            if (existingRestaurant is null)
            {
                _logger.LogWarning("Restaurant not found for update.");
                throw new NotFoundException("Restaurant not found.");
            }
            string imagePath = null;
            if (updateRestaurantDto.ImageUrl != null && updateRestaurantDto.ImageUrl.Length > 0)
            {
                imagePath = await SaveImageAsync(updateRestaurantDto.ImageUrl, webRootPath);
            }
            _mapper.Map(updateRestaurantDto, existingRestaurant);
            if (imagePath != null)
            {
                existingRestaurant.ImageUrl = imagePath;
            }
            await _unitOfWork.SaveChangesAsync();
            var restaurantDto = _mapper.Map<ReadRestaurantDto>(existingRestaurant);
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
        
        private async Task<string> SaveImageAsync(IFormFile image, string webRootPath)
        {
            string uploadsFolder = Path.Combine(webRootPath, "Images", "Restaurants");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return Path.Combine("Images", "Restaurants", uniqueFileName).Replace("\\", "/");
        }

    }
}
