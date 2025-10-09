using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Restaurants.DTOS;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Restaurants.Mapping
{
    public class RestaurantProfile :Profile
    {
        public RestaurantProfile()
        {
            CreateMap<AddRestaurantDto,Restaurant >()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
            CreateMap<Restaurant, ReadRestaurantDto>().
                ForMember(dest=>dest.ImageUrl, opt => opt.MapFrom<RestuarantPictureUrlResolver>())

                .ReverseMap();
            CreateMap<Restaurant, UpdateRestaurantDto>().ReverseMap()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

        }
    }
}
