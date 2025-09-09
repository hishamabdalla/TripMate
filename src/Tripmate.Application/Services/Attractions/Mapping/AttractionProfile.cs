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
                    opt => opt.MapFrom(src => ParseAttractionType(src.Type)))
                .ReverseMap();

        }

        private AttractionType ParseAttractionType(string typeString)
        {
            if (string.IsNullOrWhiteSpace(typeString))
                return AttractionType.Entertainment;
            return Enum.TryParse<AttractionType>(typeString, true, out var result) && Enum.IsDefined(typeof(AttractionType), result)
                ? result
                : AttractionType.Entertainment;
        }
    }
}
