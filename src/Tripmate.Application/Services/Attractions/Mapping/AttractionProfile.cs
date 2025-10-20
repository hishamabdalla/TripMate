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
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom<AttractionPictureUrlResolver>())
                .ReverseMap();


            CreateMap<SetAttractionDto, Attraction>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
               

        }
       
    }
}
