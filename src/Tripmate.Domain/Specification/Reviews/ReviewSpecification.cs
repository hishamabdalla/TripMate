using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Reviews;
using Tripmate.Domain.Entities.Models;
using Tripmate.Domain.Enums;

namespace Tripmate.Domain.Specification.Reviews
{
    public class ReviewSpecification : BaseSpecification<Review,int>
    {
        public ReviewSpecification(ReviewParameters parameters)
            : base(BuildPredicate(parameters))
        {

            AddOrderByDescending(x => x.CreatedAt);

            ApplyIncludes(parameters);
            ApplyPaging(parameters.PageNumber, parameters.PageSize);

        }

        public ReviewSpecification(int id,ReviewType reviewType):
            base(x => x.Id == id)
        {
            AddInclude(x => x.User);
            AddOrderByDescending(x => x.CreatedAt);
            if (reviewType == ReviewType.Attraction)
                AddInclude(x => x.Attraction);
            else if (reviewType == ReviewType.Restaurant)
                AddInclude(x => x.Restaurant);
            else if (reviewType == ReviewType.Hotel)
                AddInclude(x => x.Hotel);

        }



        private static Expression<Func<Review, bool>> BuildPredicate(ReviewParameters parameters)
        {
            return x =>
                (string.IsNullOrEmpty(parameters.UserId) || x.UserId == parameters.UserId) &&
                (!parameters.ReviewType.HasValue ||
                    (parameters.ReviewType == ReviewType.Attraction && x.AttractionId == parameters.EntityId) ||
                    (parameters.ReviewType == ReviewType.Restaurant && x.RestaurantId == parameters.EntityId) ||
                    (parameters.ReviewType == ReviewType.Hotel && x.HotelId == parameters.EntityId)
                );
        }

        private void ApplyIncludes(ReviewParameters parameters)
        {
            AddInclude(x => x.User);

            if (parameters.ReviewType == ReviewType.Attraction)
                AddInclude(x => x.Attraction);
            else if (parameters.ReviewType == ReviewType.Restaurant)
                AddInclude(x => x.Restaurant);
            else if (parameters.ReviewType == ReviewType.Hotel)
                AddInclude(x => x.Hotel);
        }


    }
}
