using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Hotels.DTOS;
using Tripmate.Application.Services.Image;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Hotels.Mapping
{
    public class HotelMappingProfile:Profile
    {
        public HotelMappingProfile()
        {
            CreateMap<AddHotelDto, Hotel>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
            CreateMap<Hotel, ReadHotelDto>()
                .ForMember(dest=>dest.ImageUrl,opt=>opt.MapFrom<HotelPictureUrlResolver>())
                .ReverseMap();
            CreateMap<UpdateHotelDto, Hotel>().ReverseMap()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
        }
    }
}
