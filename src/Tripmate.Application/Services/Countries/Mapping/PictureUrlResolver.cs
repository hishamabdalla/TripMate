using AutoMapper;
using Microsoft.AspNetCore.Http;
using Tripmate.Application.Services.Countries.DTOs;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Countries.Mapping
{
    public class PictureUrlResolver(IHttpContextAccessor httpContextAccessor)
        : IValueResolver<Country, CountryDto, string>
    {
        public string Resolve(Country source, CountryDto destination, string destMember, ResolutionContext context)
        {

            if (string.IsNullOrEmpty(source.ImageUrl))
            {
                return string.Empty;
            }
            var request = httpContextAccessor.HttpContext?.Request;
            if (request == null)
            {
                return source.ImageUrl; // Return the original URL if the request is not available
            }
            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/Images/Countries/{source.ImageUrl}"; // Construct the full URL for the image

        }
    }
}
