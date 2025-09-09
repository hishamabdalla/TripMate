using AutoMapper;
using Microsoft.AspNetCore.Http;
using Tripmate.Application.Services.Attractions.DTOs;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Attractions.Mapping
{
    public class AttractionImagesUrlResolver(IHttpContextAccessor httpContextAccessor)
        : IValueResolver<Attraction, AttractionDto, string>
    {
        public string Resolve(Attraction source, AttractionDto destination, string destMember, ResolutionContext context)
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
            return $"{baseUrl}/Images/Attractions/{source.ImageUrl}"; // Construct the full URL for the image

        }
    }
}
