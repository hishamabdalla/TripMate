using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Domain.Specification.FavoriteList
{
    public class FavoriteSpecification :BaseSpecification<Favorite,int>
    {
        public FavoriteSpecification(string userId,FavoriteParameters favoriteParameters)
            :base(x=>x.UserId == userId &&( !favoriteParameters.FavoriteType.HasValue|| x.FavoriteType==favoriteParameters.FavoriteType))// Retrieve all favorites that belong to the specified user,If a favorite type is specified, return only favorites of that type.
        {
            AddInclude(x => x.User);
            AddInclude(x => x.Hotel);
            AddInclude(x => x.Restaurant);
            AddInclude(x => x.Attraction);
            AddOrderByDescending(x => x.CreatedAt);
            ApplyPaging(favoriteParameters.PageNumber, favoriteParameters.PageSize);
        }
    }
}
