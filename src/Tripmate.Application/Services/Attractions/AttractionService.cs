using AutoMapper;
using Microsoft.Extensions.Logging;
using Tripmate.Application.Services.Abstractions.Attraction;
using Tripmate.Application.Services.Attractions.DTOs;
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
        public AttractionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AttractionService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
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
        public Task<ApiResponse<AttractionDto>> AddAsync(SetAttractionDto setAttractionDto)
        {
            throw new NotImplementedException();

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
