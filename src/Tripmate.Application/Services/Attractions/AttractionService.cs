using AutoMapper;
using Microsoft.Extensions.Logging;
using Tripmate.Application.Services.Abstractions.Attraction;
using Tripmate.Application.Services.Attractions.DTOs;
using Tripmate.Application.Services.Image;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities.Models;
using Tripmate.Domain.Exceptions;
using Tripmate.Domain.Interfaces;
using Tripmate.Domain.Specification.Attractions;

namespace Tripmate.Application.Services.Attractions
{
    public class AttractionService : IAttractionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AttractionService> _logger;
        private readonly IFileService _fileService;
        public AttractionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AttractionService> logger , IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _fileService = fileService;
        }

        public async Task<ApiResponse<IEnumerable<AttractionDto>>>GetAllAttractionsAsync()
        {
            var attractions = await _unitOfWork.Repository<Attraction, int>().GetAllWithSpecAsync(new AttractionSpecification());
            if (attractions == null || !attractions.Any())
            {
                _logger.LogWarning("No attractions found.");
                throw new NotFoundException("Attractions not found.");

            }
            var attractionDtos = _mapper.Map<IEnumerable<AttractionDto>>(attractions);

         
            return new ApiResponse<IEnumerable<AttractionDto>>(attractionDtos)
            {
                Success = true,
                StatusCode = 200, // OK
                Message = "Attractions retrieved successfully."
            };


        }

        public async Task<ApiResponse<AttractionDto>> GetAttractionByIdAsync(int id)
        {
            var spec = new AttractionSpecification(id);
            var attraction = await _unitOfWork.Repository<Attraction, int>().GetByIdWithSpecAsync(spec);
            if (attraction == null)
            {
                _logger.LogWarning("Attraction with ID {Id} not found.", id);
                throw new NotFoundException($"Attraction with ID {id} not found.");
            }
            var attractionDto = _mapper.Map<AttractionDto>(attraction);
            _logger.LogInformation("Attraction with ID {Id} retrieved successfully.", id);
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
                _logger.LogError("Attempted to add a null attraction.");
                throw new BadRequestException("Attraction data cannot be null.");

            }

            var attraction = _mapper.Map<Attraction>(setAttractionDto);
            if (setAttractionDto.ImageUrl == null)
            {
                _logger.LogError("ImageUrl is required for adding an attraction.");
                throw new BadRequestException("ImageUrl is required.");
            }
            // Handle image upload

            var imageUrl = await _fileService.UploadImageAsync(setAttractionDto.ImageUrl, "attractions");
            attraction.ImageUrl = imageUrl;
            await _unitOfWork.Repository<Attraction, int>().AddAsync(attraction);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Attraction with ID {Id} added successfully.", attraction.Id);

            var attractionDto = _mapper.Map<AttractionDto>(attraction);
            return new ApiResponse<AttractionDto>(attractionDto)
            {
                Success = true,
                StatusCode = 201, // Created
                Message = "Attraction added successfully."
            };

        }


        public Task<ApiResponse<AttractionDto>> Update(int id, SetAttractionDto attractionDto)
        {
            throw new NotImplementedException();

        }
        public Task<ApiResponse<bool>> Delete(int id)
        {
            throw new NotImplementedException();
        }

    }
}
