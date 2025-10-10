using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Image.ImagesFolders;
using Tripmate.Application.Services.Image.PictureResolver;
using Tripmate.Application.Services.Restaurants.DTOS;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Restaurants.Mapping
{
    public class RestuarantPictureUrlResolver(IHttpContextAccessor httpContextAccessor):
        GenericPictureUrlResolver<Restaurant,ReadRestaurantDto>(httpContextAccessor, FoldersNames.Restaurants)
    {
    }
}
