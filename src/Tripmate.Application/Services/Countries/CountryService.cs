using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Abstractions.Country;
using Tripmate.Application.Services.Countries.DTOs;
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
        public CountryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CountryService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public Task<ApiResponse<CountryDto>> AddAsync(SetCountryDto setCountryDto)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<CountryDto>> DeleteAsync(int id)
        {
            throw new NotImplementedException();
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

        public Task<ApiResponse<CountryDto>> GetCountryByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<CountryDto>> UpdateAsync(int id, CountryDto countryDto)
        {
            throw new NotImplementedException();
        }

       


    }
}
