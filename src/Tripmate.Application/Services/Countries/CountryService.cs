using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Abstractions.Country;
using Tripmate.Application.Services.Countries.DTOs;
using Tripmate.Application.Services.Image;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities.Models;
using Tripmate.Domain.Exceptions;
using Tripmate.Domain.Interfaces;
using Tripmate.Domain.Specification.Countries;

namespace Tripmate.Application.Services.Countries
{
    public class CountryService : ICountryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CountryService> _logger;
        private readonly IFileService _fileService;
        public CountryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CountryService> logger, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _fileService = fileService;
        }

        public async Task<ApiResponse<CountryDto>> AddAsync(SetCountryDto setCountryDto)
        {

            if (setCountryDto == null)
            {
                _logger.LogError("Attempted to add a null country.");
                throw new BadRequestException("Country data cannot be null.");
            }


            var country = _mapper.Map<Country>(setCountryDto);
           
            
            if (setCountryDto.ImageUrl == null)
            {
                _logger.LogError("ImageUrl is required for adding a country.");
                throw new BadRequestException("ImageUrl is required.");

            }
            // Handle image upload
            var imageUrl = await _fileService.UploadImageAsync(setCountryDto.ImageUrl, "countries");
            country.ImageUrl = imageUrl;


            await _unitOfWork.Repository<Country, int>().AddAsync(country);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Country with ID {Id} added successfully.", country.Id);
            var countryDto = _mapper.Map<CountryDto>(country);
            return new ApiResponse<CountryDto>(countryDto)
            {
                Message = "Country added successfully.",
                StatusCode = 201 // Created
            };

        }


       

        public async Task<ApiResponse<IEnumerable<CountryDto>>> GetAllCountriesAsync()

        {
            var spec= new CountrySpecification();
            var countries = await _unitOfWork.Repository<Country,int>().GetAllWithSpecAsync(spec);
            if (countries == null || !countries.Any())
            {
                _logger.LogWarning("No countries found.");
                throw new NotFoundException("No countries found.");

            }

            var countryDtos = _mapper.Map<IEnumerable<CountryDto>>(countries);
            _logger.LogInformation("Retrieved {Count} countries.", countryDtos.Count());

            return new ApiResponse<IEnumerable<CountryDto>>(true, 200, "Countries retrieved successfully.", countryDtos);

        }

        public async Task<ApiResponse<CountryDto>> GetCountryByIdAsync(int id)
        {
            var country = await _unitOfWork.Repository<Country, int>().GetByIdWithSpecAsync(new CountrySpecification(id));
            if(country == null)
            {
                _logger.LogWarning("Country with ID {Id} not found.", id);
                throw new NotFoundException("Country", id.ToString());
            }

            var countryDto=_mapper.Map<CountryDto>(country);
            _logger.LogInformation("Country with ID {Id} retrieved successfully.", id);

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
                _logger.LogError("Attempted to update a null country.");
                throw new BadRequestException("Country data cannot be null.");
            }

            var existingCountry = await _unitOfWork.Repository<Country, int>().GetByIdAsync(id);
            if (existingCountry == null)
            {
                _logger.LogWarning("Country with ID {Id} not found for update.", id);
                throw new NotFoundException("Country", id.ToString());
            }


            if (countryDto.ImageUrl != null)
            {
                if (!string.IsNullOrEmpty(existingCountry.ImageUrl))
                {
                    _fileService.DeleteImage(existingCountry.ImageUrl, "countries");
                }

                var newImageUrl = await _fileService.UploadImageAsync(countryDto.ImageUrl, "countries");
                existingCountry.ImageUrl = newImageUrl;
            }
            else
            {
                existingCountry.ImageUrl = existingCountry.ImageUrl;
            }

            // Map the other properties
            existingCountry.Name = countryDto.Name;
            existingCountry.Description = countryDto.Description;



            _unitOfWork.Repository<Country, int>().Update(existingCountry);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Country with ID {Id} updated successfully.", id);

            return new ApiResponse<CountryDto>(_mapper.Map<CountryDto>(existingCountry))
            {
                Message = "Country updated successfully.",
                StatusCode = 200 // OK
            };
        }

        public async Task<ApiResponse<bool>> Delete(int id)
        {
            var country = await _unitOfWork.Repository<Country, int>().GetByIdAsync(id);
            if (country == null)
            {
                _logger.LogWarning("Country with ID {Id} not found for deletion.", id);
                throw new NotFoundException("Country", id.ToString());

            }

            _unitOfWork.Repository<Country, int>().Delete(id);
            // Optionally, you can delete the image associated with the country
            if (!string.IsNullOrEmpty(country.ImageUrl))
            {
                _fileService.DeleteImage(country.ImageUrl, "countries");
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Country with ID {Id} deleted successfully.", id);

            return new ApiResponse<bool>(true)
            {
                Message = "Country deleted successfully.",
                StatusCode = 200 // OK
            };


        }


    }
}
