using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
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
        IEnumerable<Expression<Func<TEntity, object>>> Includes { get; }

        /// <summary>
        /// Gets the order by expression for the specification.
        /// </summary>
        Expression<Func<TEntity, object>> OrderBy { get; set; }

        /// <summary>
        /// Gets the order by descending expression for the specification.
        /// </summary>
        Expression<Func<TEntity, object>> OrderByDescending { get; set; }

    }
}
