using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Entities.Models;
using Tripmate.Domain.Enums;

namespace Tripmate.Domain.Specification.FavoriteList
{
    public class FavoriteExistsSpecification :BaseSpecification<Favorite,int>
    {
        // Specification to check if a favorite already exists for a specific user and entity.
        // Prevents adding duplicate favorites of the same type and entity.
        public FavoriteExistsSpecification(string userId,FavoriteType favoriteType,int entityId)
            :base(x=>x.UserId==userId&& x.FavoriteType== favoriteType &&
                (favoriteType == FavoriteType.Hotel && x.HotelId == entityId) ||         
                (favoriteType == FavoriteType.Restaurant && x.RestaurantId == entityId) || 
                (favoriteType == FavoriteType.Attraction && x.AttractionId == entityId)
            )
        {

        }
    }
}
