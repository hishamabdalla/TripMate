using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Entities.Base;
using Tripmate.Domain.Specification;

namespace Tripmate.Infrastructure.Specification
{
    public class SpecificationEvaluator<TEntity,TKey> where TEntity :BaseEntity<TKey>
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity, TKey> specification)
        {
            var query = inputQuery.AsQueryable();

            if(specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            
            query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }
            return query;

        }

    }
}
