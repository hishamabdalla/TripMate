using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.FavoriteList.DTOS;
using Tripmate.Domain.Entities.Models;
using Tripmate.Domain.Enums;

namespace Tripmate.Application.Services.FavoriteList.Mapping
{
    public class FavoriteMappingProfile :Profile
    {
        public FavoriteMappingProfile()
        {
            CreateMap<AddFavoriteDto, Favorite>()
                // If the favorite type doesn't match this entity type, set the ID to null (as nullable int).
                // (int?)null means "a null value of type int?" to match the property type.
                .ForMember(dest => dest.HotelId, opt => opt.MapFrom(src => src.FavoriteType == FavoriteType.Hotel ? src.EntityId : (int?)null))
                .ForMember(dest => dest.RestaurantId, opt => opt.MapFrom(src => src.FavoriteType == FavoriteType.Restaurant ? src.EntityId : (int?)null))
                .ForMember(dest => dest.AttractionId, opt => opt.MapFrom(src => src.FavoriteType == FavoriteType.Attraction ? src.EntityId : (int?)null));

            CreateMap<Favorite, FavoriteResponseDto>()
                // Choose which ID to send based on the favorite type:
                // Hotel → use HotelId, Restaurant → use RestaurantId, Attraction → use AttractionId
                .ForMember(dest => dest.Id, opt => opt.MapFrom
                (src => src.FavoriteType==FavoriteType.Hotel ? src.HotelId :
                src.FavoriteType==FavoriteType.Restaurant ? src.RestaurantId : src.AttractionId));
        }
    }
}
