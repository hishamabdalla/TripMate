using AutoMapper;
using Microsoft.Extensions.Logging;
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

        public RegionService(IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<RegionService> logger,
            IFileService fileService)
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

        public async Task<ApiResponse<RegionDto>> CreateRegionAsync(SetRegionDto setRegionDto)
        {
            if (setRegionDto == null)
            {
                _logger.LogError("Attempted to create a null region.");
                throw new BadRequestException("Region data cannot be null");
            }

            var country = await _unitOfWork.Repository<Country, int>().GetByIdAsync(setRegionDto.CountryId);

            if (country == null)
            {
                _logger.LogError($"Country with ID {setRegionDto.CountryId} not found.");
                throw new NotFoundException($"Country with ID {setRegionDto.CountryId} not found.");

            }


            var region = _mapper.Map<Region>(setRegionDto);




            if (setRegionDto.ImageUrl == null)
            {
                _logger.LogError("ImageUrl is required for adding a Region");
                throw new BadRequestException("ImageUrl is Required");
            }

            var imageUrl = await _fileService.UploadImageAsync(setRegionDto.ImageUrl, "Regions");

            region.ImageUrl = imageUrl;

            await _unitOfWork.Repository<Region, int>().AddAsync(region);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Region With ID {region.Id} Added Successfully.");

            var regionExists = await _unitOfWork.Repository<Region, int>().GetByIdWithSpecAsync(new RegionSpecification(region.Id));
            var regionDto = _mapper.Map<RegionDto>(regionExists);

            return new ApiResponse<RegionDto>(regionDto)
            {
                Message = "Region added successfully.",
                StatusCode = 201 // Created
            };

        }

        public async Task<ApiResponse<RegionDto>> UpdateRegionAsync(int id, SetRegionDto setRegionDto)
        {

            if (setRegionDto == null)
            {
                _logger.LogError("Attempted to update a null region.");
                throw new BadRequestException("Region data cannot be null");
            }
            var existRegion = await _unitOfWork.Repository<Region, int>().GetByIdAsync(id);

            if (existRegion == null)
            {
                _logger.LogWarning("Region with ID {Id} not found for update.", id);
                throw new NotFoundException($"Region With Id {id} not found.");

            }
            var region = _mapper.Map<Region>(setRegionDto);

            if (setRegionDto.ImageUrl != null)
            {
                if (!string.IsNullOrEmpty(existRegion.ImageUrl))
                {
                    _fileService.DeleteImage(existRegion.ImageUrl, $"{nameof(Region)}s");
                }

                var imageUrl = await _fileService.UploadImageAsync(setRegionDto.ImageUrl, "Regions");

                existRegion.ImageUrl = imageUrl;
            }

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"Region with ID {id} updated successfully.");


            var regionResponse = _mapper.Map<RegionDto>(region);

            return new ApiResponse<RegionDto>(regionResponse)
            {
                Message = "Region updated successfully.",
                StatusCode = 200 // OK
            };

        }

        public async Task<ApiResponse<bool>> DeleteRegionAsync(int id)
        {
            var existRegion = await _unitOfWork.Repository<Region, int>().GetByIdAsync(id);
            if (existRegion == null)
            {
                _logger.LogWarning("Region with ID {Id} not found for deletion.", id);
                throw new NotFoundException($"Region With Id {id} not found.");
            }
            _unitOfWork.Repository<Region, int>().Delete(existRegion);

            await _unitOfWork.SaveChangesAsync();

            if (!string.IsNullOrEmpty(existRegion.ImageUrl))
            {
                _fileService.DeleteImage(existRegion.ImageUrl, "Regions");
            }

            _logger.LogInformation($"Region with ID {id} deleted successfully.");
            return new ApiResponse<bool>(true)
            {
                Message = "Region deleted successfully.",
                StatusCode = 200 // OK
            };

        }
        public async Task<PaginationResponse<IEnumerable<RegionDto>>> GetRegionsAsync(RegionParameters parameters)
        {
            if (parameters.PageNumber <= 0)
                throw new BadRequestException("PageNumber must be greater than 0.");

            if (parameters.PageSize <= 0)
                throw new BadRequestException("PageSize must be greater than 0.");

            var dataSpec = new RegionSpecification(parameters);
            var countSpec = new RegionForCountingSpecification(parameters);
            var regions = await _unitOfWork.Repository<Region, int>().GetAllWithSpecAsync(dataSpec);
            var totalCount = await _unitOfWork.Repository<Region, int>().CountAsync(countSpec);

            if (regions == null || !regions.Any())
            {
                _logger.LogWarning("No regions found matching the provided criteria.");
                throw new NotFoundException("No regions found matching the provided criteria.");
            }
            var regionDtos = _mapper.Map<IEnumerable<RegionDto>>(regions);

            return new PaginationResponse<IEnumerable<RegionDto>> (regionDtos, totalCount, parameters.PageNumber,
                parameters.PageSize);
        }
    }
}
