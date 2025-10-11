using AutoMapper;
using Microsoft.Extensions.Logging;
using Tripmate.Application.Services.Abstractions.Attraction;
using Tripmate.Application.Services.Attractions.DTOs;
using Tripmate.Application.Services.Countries.DTOs;
using Tripmate.Application.Services.Image;
using Tripmate.Application.Services.Image.ImagesFolders;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities.Models;
using Tripmate.Domain.Exceptions;
using Tripmate.Domain.Interfaces;
using Tripmate.Domain.Specification.Attractions;
using Tripmate.Domain.Specification.Countries;

namespace Tripmate.Application.Services.Attractions
{
    public class AttractionService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<AttractionService> logger,
        IFileService fileService)
        : IAttractionService
    {
       
        public async Task<PaginationResponse<IEnumerable<AttractionDto>>> GetAttractionsAsync(AttractionParameter parameters)
        {
            if (parameters.PageNumber <= 0)
                throw new BadRequestException("PageNumber must be greater than 0.");

            if (parameters.PageSize <= 0)
                throw new BadRequestException("PageSize must be greater than 0.");

            var dataSpec = new AttractionSpecification(parameters);
            var countSpec = new AttractionsForCountingSpecification(parameters);
            var Attractions = await unitOfWork.Repository<Attraction, int>().GetAllWithSpecAsync(dataSpec);
            var totalCount = await unitOfWork.Repository<Attraction, int>().CountAsync(countSpec);

            if (!Attractions.Any())
            {
                logger.LogInformation("No Attractions found.");
                throw new NotFoundException("No Attractions found.");

            }
            var AttractionsDtos  = mapper.Map<IEnumerable<AttractionDto>>(Attractions);

            return new PaginationResponse<IEnumerable<AttractionDto>>(AttractionsDtos, totalCount, parameters.PageNumber,
                parameters.PageSize);
        }

        public async Task<ApiResponse<AttractionDto>> GetAttractionByIdAsync(int id)
        {
            var spec = new AttractionSpecification(id);
            var attraction = await unitOfWork.Repository<Attraction, int>().GetByIdWithSpecAsync(spec);
            if (attraction == null)
            {
                logger.LogWarning("Attraction with ID {Id} not found.", id);
                throw new NotFoundException($"Attraction with ID {id} not found.");
            }
            var attractionDto = mapper.Map<AttractionDto>(attraction);
            logger.LogInformation("Attraction with ID {Id} retrieved successfully.", id);
            return new ApiResponse<AttractionDto>(attractionDto)
            {
                Success = true,
                StatusCode = 200, // OK
                Message = "Attraction retrieved successfully."
            };


        }
        public async Task<ApiResponse<AttractionDto>> AddAsync(SetAttractionDto setAttractionDto)
        {
            if (setAttractionDto == null)
            {
                logger.LogError("Attempted to add a null attraction.");
                throw new BadRequestException("Attraction data cannot be null.");

            }

            var attraction = mapper.Map<Attraction>(setAttractionDto);
            if (setAttractionDto.ImageUrl == null)
            {
                logger.LogError("ImageUrl is required for adding an attraction.");
                throw new BadRequestException("ImageUrl is required.");
            }

            var Region = await unitOfWork.Repository<Region, int>().GetByIdAsync(setAttractionDto.RegionId);
            if (Region == null)
            {
                logger.LogError("Region with ID {RegionId} not found.", setAttractionDto.RegionId);
                throw new NotFoundException($"Region with ID {setAttractionDto.RegionId} not found.");
            }

            // Handle image upload

            var imageUrl = await fileService.UploadImageAsync(setAttractionDto.ImageUrl, FoldersNames.Attractions);
            attraction.ImageUrl = imageUrl;

           

            

            await unitOfWork.Repository<Attraction, int>().AddAsync(attraction);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Attraction with ID {Id} added successfully.", attraction.Id);

            var attractionDto = mapper.Map<AttractionDto>(attraction);
            return new ApiResponse<AttractionDto>(attractionDto)
            {
                Success = true,
                StatusCode = 201, // Created
                Message = "Attraction added successfully."
            };

        }


        public async Task<ApiResponse<AttractionDto>> UpdateAsync(int id, SetAttractionDto attractionDto)
        {
            if (attractionDto == null)
            {
                logger.LogError("Attempted to update a null attraction.");
                throw new BadRequestException("Attraction data cannot be null.");
            }
            var existingAttraction = await unitOfWork.Repository<Attraction, int>().GetByIdAsync(id);
            if (existingAttraction == null)
            {
                logger.LogWarning("Attraction with ID {Id} not found for update.", id);
                throw new NotFoundException($"Attraction with ID {id} not found.");
            }

           
            if (attractionDto.ImageUrl != null && attractionDto.ImageUrl?.Length > 0)
            {
               if (!string.IsNullOrEmpty(existingAttraction.ImageUrl))
                {
                    // Delete the old image if it exists
                     fileService.DeleteImage(existingAttraction.ImageUrl,FoldersNames.Attractions);
                }
                // Handle image upload
                var imageUrl = await fileService.UploadImageAsync(attractionDto.ImageUrl, FoldersNames.Attractions);

                existingAttraction.ImageUrl = imageUrl;

            }

            var region = await unitOfWork.Repository<Region, int>().GetByIdAsync(attractionDto.RegionId);
            if (region == null)
            {
                logger.LogError("Region with ID {RegionId} not found.", attractionDto.RegionId);
                throw new NotFoundException($"Region with ID {attractionDto.RegionId} not found.");
            }

            mapper.Map(attractionDto, existingAttraction);
            



            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Attraction with ID {Id} updated successfully.", id);

            var attractionResponse = mapper.Map<AttractionDto>(existingAttraction);
            return new ApiResponse<AttractionDto>(attractionResponse)
            {
                Success = true,
                StatusCode = 200, // OK
                Message = "Attraction updated successfully."
            };

        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var attraction = await unitOfWork.Repository<Attraction, int>().GetByIdAsync(id);
            if (attraction == null)
            {
                logger.LogWarning("Attraction with ID {Id} not found for deletion.", id);
                throw new NotFoundException($"Attraction with ID {id} not found.");
            }
             unitOfWork.Repository<Attraction, int>().Delete(attraction);

            if (!string.IsNullOrEmpty(attraction.ImageUrl))
            {
                fileService.DeleteImage(attraction.ImageUrl, "Attractions");
            }


                await unitOfWork.SaveChangesAsync();
            logger.LogInformation("Attraction with ID {Id} deleted successfully.", id);
            return new ApiResponse<bool>(true)
            {
                Success = true,
                StatusCode = 200, // OK
                Message = "Attraction deleted successfully."
            };

        }

     
    }
}
