using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Reviews.DTOs;
using Tripmate.Domain.Entities.Models;
using Tripmate.Domain.Enums;

namespace Tripmate.Application.Services.Reviews.Mapping
{
    public class ReviewProfile:Profile
    {
        public ReviewProfile()
        {
            CreateMap<AddReviewDto, Review>()
              .ForMember(dest => dest.ReviewDate, opt => opt.MapFrom(src => DateTime.UtcNow.ToString("dd MMMM yyyy HH:mm:ss")))
              .ForMember(dest => dest.AttractionId, opt => opt.MapFrom(src =>
                  src.ReviewType == ReviewType.Attraction ? (int?)src.EntityId : null))
              .ForMember(dest => dest.RestaurantId, opt => opt.MapFrom(src =>
                  src.ReviewType == ReviewType.Restaurant ? (int?)src.EntityId : null))
              .ForMember(dest => dest.HotelId, opt => opt.MapFrom(src =>
                  src.ReviewType == ReviewType.Hotel ? (int?)src.EntityId : null));
              
            CreateMap<Review, ReadReviewDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ReviewType, opt => opt.MapFrom(src =>
                    src.AttractionId.HasValue ? ReviewType.Attraction :
                    src.RestaurantId.HasValue ? ReviewType.Restaurant :
                     ReviewType.Hotel))
                .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src =>
                    src.AttractionId ?? src.RestaurantId ?? src.HotelId ?? 0))
                .ForMember(dest => dest.EntityName, opt => opt.MapFrom(src =>
                    src.Attraction != null ? src.Attraction.Name :
                    src.Restaurant != null ? src.Restaurant.Name :
                    src.Hotel != null ? src.Hotel.Name : string.Empty))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src =>src.ReviewDate.ToString("dd MMMM yyyy HH:mm:ss")));

            CreateMap<UpdateReviewDto, Review>();


        }
    }
}
