using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Countries.DTOs;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Countries.Mapping
{
    public class CountryProfile:Profile
    {
        public CountryProfile()
        {
            CreateMap<Country, CountryDto>()
                .ReverseMap();

            CreateMap<SetCountryDto,Country>().ReverseMap();


        }
    }
}
