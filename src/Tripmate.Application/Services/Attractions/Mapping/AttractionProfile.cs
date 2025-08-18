using AutoMapper;
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
    public class AttractionProfile:Profile
    {
        public AttractionProfile()
        {
            CreateMap<Attraction, AttractionDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom<AttractionImagesUrlResolver>())
                .ReverseMap();


            CreateMap<SetAttractionDto, Attraction>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                .ReverseMap();



        }
    }
}
