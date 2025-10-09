using AutoMapper;
using Microsoft.AspNetCore.Http;
using Tripmate.Application.Services.Countries.DTOs;
using Tripmate.Application.Services.Image.ImagesFolders;
using Tripmate.Application.Services.Image.PictureResolver;
using Tripmate.Domain.Entities.Models;
using Tripmate.Domain.Enums;

namespace Tripmate.Application.Services.Countries.Mapping
{
    public class CountryPictureUrlResolver : GenericPictureUrlResolver<Country, CountryDto >
    {
        public CountryPictureUrlResolver(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor, FoldersNames.Countries)
        {
        }

    }
}