using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Attractions.DTOs;
using Tripmate.Application.Services.Countries.DTOs;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Attractions.Mapping
{
    public class AttractionImagesUrlResolver : IValueResolver<Attraction, AttractionDto, string>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AttractionImagesUrlResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string Resolve(Attraction source, AttractionDto destination, string destMember, ResolutionContext context)
        {

            if (string.IsNullOrEmpty(source.ImageUrl))
            {
                return string.Empty;
            }
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
            {
                return source.ImageUrl; // Return the original URL if the request is not available
            }
            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/Images/Attractions/{source.ImageUrl}"; // Construct the full URL for the image

        }
    }
}
