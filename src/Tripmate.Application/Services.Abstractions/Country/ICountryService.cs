using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Countries.DTOs;
using Tripmate.Domain.Common.Response;

namespace Tripmate.Application.Services.Abstractions.Country
{
    public interface ICountryService
    {
        Task<ApiResponse<IEnumerable<CountryDto>>> GetAllCountriesAsync();
        Task<ApiResponse<CountryDto>> GetCountryByIdAsync(int id);
        Task<ApiResponse<CountryDto>> AddAsync(SetCountryDto setCountryDto);
        Task<ApiResponse<CountryDto>> UpdateAsync(int id, CountryDto countryDto);
        Task<ApiResponse<CountryDto>> DeleteAsync(int id);


    }
}
