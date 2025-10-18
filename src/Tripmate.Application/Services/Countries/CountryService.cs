using AutoMapper;
using Microsoft.Extensions.Logging;
using Tripmate.Application.Services.Abstractions.Country;
using Tripmate.Application.Services.Countries.DTOs;
using Tripmate.Application.Services.Image;
using Tripmate.Application.Services.Image.ImagesFolders;
using Tripmate.Application.Services.Regions.DTOs;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities.Models;
using Tripmate.Domain.Enums;
using Tripmate.Domain.Exceptions;
using Tripmate.Domain.Interfaces;
using Tripmate.Domain.Specification.Countries;
using Tripmate.Domain.Specification.Regions;

namespace Tripmate.Application.Services.Countries
{
    public class CountryService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CountryService> logger,
        IFileService fileService)
        : ICountryService
    {
        public async Task<ApiResponse<CountryDto>> AddAsync(SetCountryDto setCountryDto)
        {
            if (setCountryDto == null)
            {
                logger.LogError("Attempted to add a null country.");
                throw new BadRequestException("Country data cannot be null.");
            }


            var country = mapper.Map<Country>(setCountryDto);

            if (setCountryDto.ImageUrl == null)
            {
                logger.LogError("ImageUrl is required for adding a country.");
                throw new BadRequestException("ImageUrl is required.");

            }
            // Handle image upload
            var imageUrl = await fileService.UploadImageAsync(setCountryDto.ImageUrl,FoldersNames.Countries);
            country.ImageUrl = imageUrl;

            await unitOfWork.Repository<Country, int>().AddAsync(country);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("Country with ID {Id} added successfully.", country.Id);
            var countryDto = mapper.Map<CountryDto>(country);
            return new ApiResponse<CountryDto>(countryDto)
            {
                Message = "Country added successfully.",
                StatusCode = 201 // Created
            };
        }
        public async Task<PaginationResponse<IEnumerable<CountryDto>>> GetCountriesAsync(CountryParameters parameters)
        {
            if (parameters.PageNumber <= 0)
                throw new BadRequestException("PageNumber must be greater than 0.");

            if (parameters.PageSize <= 0)
                throw new BadRequestException("PageSize must be greater than 0.");

            var dataSpec = new CountrySpecification(parameters);
            var countSpec = new CountriesForCountingSpecification(parameters);
            var countries = await unitOfWork.Repository<Country, int>().GetAllWithSpecAsync(dataSpec);
            var totalCount = await unitOfWork.Repository<Country, int>().CountAsync(countSpec);

            if(!countries.Any())
            {
                logger.LogWarning("No countries found matching the provided criteria.");
                throw new NotFoundException("No countries found matching the provided criteria.");
            }
            var countryDtos = mapper.Map<IEnumerable<CountryDto>>(countries).ToList();

            return new PaginationResponse<IEnumerable<CountryDto>>(countryDtos, totalCount, parameters.PageNumber,
                parameters.PageSize);
        }
        public async Task<ApiResponse<CountryDto>> GetCountryByIdAsync(int id)
        {
            var country = await unitOfWork.Repository<Country, int>().GetByIdWithSpecAsync(new CountrySpecification(id));
            if(country == null)
            {
                logger.LogWarning("Country with ID {Id} not found.", id);
                throw new NotFoundException("Country", id.ToString());
            }

            var countryDto=mapper.Map<CountryDto>(country);
            logger.LogInformation("Country with ID {Id} retrieved successfully.", id);

            return new ApiResponse<CountryDto>(countryDto)
            {
                Message = "Country retrieved successfully.",
                StatusCode = 200
            };

        }

        public async Task<ApiResponse<CountryDto>> Update(int id, SetCountryDto countryDto)
        {
            if (countryDto == null)
            {
                logger.LogError("Attempted to update a null country.");
                throw new BadRequestException("Country data cannot be null.");
            }

           
            var existingCountry = await unitOfWork.Repository<Country, int>().GetByIdAsync(id);
            if (existingCountry == null)
            {
                logger.LogWarning("Country with ID {Id} not found for update.", id);
                throw new NotFoundException("Country", id.ToString());
            }

            if (countryDto.ImageUrl != null)
            {
                if (!string.IsNullOrEmpty(existingCountry.ImageUrl))
                {
                    fileService.DeleteImage(existingCountry.ImageUrl,FoldersNames.Countries);
                }

                var newImageUrl = await fileService.UploadImageAsync(countryDto.ImageUrl, FoldersNames.Countries);
                existingCountry.ImageUrl = newImageUrl;
            }
            

            // Map the other properties
            mapper.Map(countryDto, existingCountry);

            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Country with ID {Id} updated successfully.", id);

            return new ApiResponse<CountryDto>(mapper.Map<CountryDto>(existingCountry))
            {
                Message = "Country updated successfully.",
                StatusCode = 200 // OK
            };
        }

        public async Task<ApiResponse<bool>> Delete(int id)
        {
            var country = await unitOfWork.Repository<Country, int>().GetByIdAsync(id);
            if (country == null)
            {
                logger.LogWarning("Country with ID {Id} not found for deletion.", id);
                throw new NotFoundException("Country", id.ToString());

            }

            unitOfWork.Repository<Country, int>().Delete(country);
            // Optionally, you can delete the image associated with the country
            if (!string.IsNullOrEmpty(country.ImageUrl))
            {
                fileService.DeleteImage(country.ImageUrl, "countries");
            }

            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Country with ID {Id} deleted successfully.", id);

            return new ApiResponse<bool>(true)
            {
                Message = "Country deleted successfully.",
                StatusCode = 200 // OK
            };


        }


    }
}
