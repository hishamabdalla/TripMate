using AutoMapper;
using Microsoft.AspNetCore.Http;
using Tripmate.Application.Services.Attractions.DTOs;
using Tripmate.Application.Services.Image.ImagesFolders;
using Tripmate.Application.Services.Image.PictureResolver;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Attractions.Mapping
{
    public class AttractionPictureUrlResolver(IHttpContextAccessor httpContextAccessor):
        GenericPictureUrlResolver<Attraction, AttractionDto>(httpContextAccessor, FoldersNames.Attractions)
    {

        
    }
}
