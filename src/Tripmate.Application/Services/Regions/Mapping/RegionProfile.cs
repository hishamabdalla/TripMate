using AutoMapper;
using Tripmate.Application.Services.Regions.DTOs;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Regions.Mapping
{
    public class RegionProfile:Profile
    {
        public RegionProfile()
        {
            CreateMap<Region, RegionDto>().
                ForMember(dest => dest.Country, opts => opts.MapFrom(src => src.Country.Name))
                .ForMember(dest => dest.ImageUrl, opts => opts.MapFrom<RegionImagesUrlResolver>());

            CreateMap<SetRegionDto, Region>()
                .ForMember(dest => dest.ImageUrl, opts => opts.Ignore());

        }
    }
}
