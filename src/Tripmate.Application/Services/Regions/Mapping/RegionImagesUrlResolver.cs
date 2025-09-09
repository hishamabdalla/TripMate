using AutoMapper;
using Microsoft.AspNetCore.Http;
using Tripmate.Application.Services.Regions.DTOs;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Regions.Mapping
{
    public class RegionImagesUrlResolver(IHttpContextAccessor httpContextAccessor)
        : IValueResolver<Region, DTOs.RegionDto, string>
    {
        public string Resolve(Region source, RegionDto destination, string destMember, ResolutionContext context)
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
            return $"{baseUrl}/Images/Regions/{source.ImageUrl}"; // Construct the full URL for the image

        }
    }
}
