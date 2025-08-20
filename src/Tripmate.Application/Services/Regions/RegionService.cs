using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Abstractions.Region;
using Tripmate.Application.Services.Image;
using Tripmate.Application.Services.Regions.DTOs;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities.Models;
using Tripmate.Domain.Exceptions;
using Tripmate.Domain.Interfaces;
using Tripmate.Domain.Specification.Countries;
using Tripmate.Domain.Specification.Regions;

namespace Tripmate.Application.Services.Regions
{
    public class RegionService : IRegionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RegionService> _logger;
        private readonly IFileService _fileService;
        public RegionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RegionService> logger , IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _fileService = fileService;
        }
        public async Task<ApiResponse<IEnumerable<RegionDto>>> GetAllRegionForCountryAsync(int countryId)
        {
            var spec = new CountrySpecification(countryId);
            var country = await _unitOfWork.Repository<Country, int>().GetByIdWithSpecAsync(spec);
            if (country == null)
            {
                _logger.LogError($"Country with ID {countryId} not found.");
                throw new NotFoundException($"Country with ID {countryId} not found.");
            }
            var regions = country.Region;
            if (regions == null || !regions.Any())
            {
                _logger.LogWarning($"No regions found for country with ID {countryId}.");
                throw new NotFoundException($"No regions found for country with ID {countryId}.");
            }
            var regionDtos = _mapper.Map<IEnumerable<RegionDto>>(regions);

            _logger.LogInformation($"Successfully retrieved {regionDtos.Count()} regions for country with ID {countryId}.");

            return new ApiResponse<IEnumerable<RegionDto>>(regionDtos)
            {
                Message = "Regions retrieved successfully.",
                Success = true
            };



        }
        public async Task<ApiResponse<RegionDto>> GetRegionByIdAsync(int regionId)
        {
            var region = await _unitOfWork.Repository<Region, int>().GetByIdWithSpecAsync(new RegionSpecification(regionId));

            if (region == null)
            {
                _logger.LogWarning($"Region with Id {regionId} not found.");
                throw new NotFoundException($"Region with Id {regionId} not found.");
            }

            var regionDto = _mapper.Map<RegionDto>(region);

            _logger.LogInformation($"Successfully retrieved region with Id {regionId}.");
            return new ApiResponse<RegionDto>(regionDto)
            {
                Message = "Region retrieved successfully.",
                Success = true
            };

        }

     


    }
}
