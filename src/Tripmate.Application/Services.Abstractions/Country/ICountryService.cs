using Tripmate.Application.Services.Countries.DTOs;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Specification.Countries;

namespace Tripmate.Application.Services.Abstractions.Country
{
    public interface ICountryService
    {
        Task<PaginationResponse<IEnumerable<CountryDto>>> GetCountriesAsync(CountryParameters parameters);
        Task<ApiResponse<CountryDto>> GetCountryByIdAsync(int id);
        Task<ApiResponse<CountryDto>> AddAsync(SetCountryDto setCountryDto);
        Task<ApiResponse<CountryDto>> Update(int id, SetCountryDto countryDto);
        Task<ApiResponse<bool>> Delete(int id);


    }
}
