using Tripmate.Application.Services.Countries.DTOs;
using Tripmate.Domain.Common.Response;

namespace Tripmate.Application.Services.Abstractions.Country
{
    public interface ICountryService
    {
        Task<ApiResponse<IEnumerable<CountryDto>>> GetAllCountriesAsync();
        Task<ApiResponse<CountryDto>> GetCountryByIdAsync(int id);
        Task<ApiResponse<CountryDto>> AddAsync(SetCountryDto setCountryDto);
        Task<ApiResponse<CountryDto>> Update(int id, SetCountryDto countryDto);
        Task<ApiResponse<bool>> Delete(int id);


    }
}
