using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Tripmate.Application.Services.Countries.DTOs;
using Tripmate.Application.Services.Image;
using Tripmate.Application.Services.Image.enums;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Countries.Mapping
{
    public class CountryProfile:Profile
    {

        public CountryProfile()
        {

            CreateMap<Country, CountryDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom<PictureUrlResolver>());
                ;
            CreateMap<SetCountryDto,Country>().ReverseMap();


        }
    }
}
