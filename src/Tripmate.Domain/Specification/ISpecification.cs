using System.Linq.Expressions;
using Tripmate.Domain.Entities.Base;

namespace Tripmate.Domain.Specification
{
    public interface ISpecification<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        /// <summary>
        /// Gets the criteria expression for the specification.
        /// </summary>
        Expression<Func<TEntity, bool>> Criteria { get; }

        /// <summary>
        /// Gets the include properties for the specification.
        /// </summary>
        List<Expression<Func<TEntity, object>>> Includes { get; }

        /// <summary>
        /// Gets the order by expression for the specification.
        /// </summary>
        Expression<Func<TEntity, object>> OrderBy { get; set; }

        /// <summary>
        /// Gets the order by descending expression for the specification.
        /// </summary>
        Expression<Func<TEntity, object>> OrderByDescending { get; set; }
        int Skip { get; set; }
        int Take { get; set; }
        public bool IsPagingEnabled { get; set; }


    }
}
