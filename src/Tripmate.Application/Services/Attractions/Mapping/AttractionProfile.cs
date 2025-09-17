using AutoMapper;
using Tripmate.Application.Services.Attractions.DTOs;
using Tripmate.Domain.Entities.Models;
using Tripmate.Domain.Enums;

namespace Tripmate.Application.Services.Attractions.Mapping
{
    public class AttractionProfile:Profile
    {
        public AttractionProfile()
        {
            CreateMap<Attraction, AttractionDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom<AttractionImagesUrlResolver>())
                .ReverseMap();


            CreateMap<SetAttractionDto, Attraction>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Type,
                    opt => opt.MapFrom(src => ParseAttractionType(src.Type))) // Use the helper method to parse the enum with a default value
                .ReverseMap();

        }

        // Helper method to parse the AttractionType enum with a default value
        // This ensures that if the input string is invalid or null, it defaults to AttractionType.Entertainment
        // This method is used in the mapping configuration above
        // The parsing is case-insensitive and trims whitespace
        // If the string does not match any enum value, it returns the default

        private AttractionType ParseAttractionType(string typeString)
        {
            if (string.IsNullOrWhiteSpace(typeString))
                return AttractionType.Entertainment;
            return Enum.TryParse<AttractionType>(typeString, true, out var result) && Enum.IsDefined(typeof(AttractionType), result) // Check if parsed value is defined in the enum

                ? result
                : AttractionType.Entertainment;
        }
    }
}
