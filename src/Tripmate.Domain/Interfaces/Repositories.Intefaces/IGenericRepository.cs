using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Entities.Base;
using Tripmate.Domain.Specification;

namespace Tripmate.Domain.Interfaces.Repositories.Intefaces
{
    public interface IGenericRepository<TEntity,TKey> where TEntity : BaseEntity<TKey>
    {
        Task<TEntity> GetByIdAsync(TKey id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TKey id);

        //specification methods

        Task<IEnumerable<TEntity>> GetAllWithSpecAsync(ISpecification<TEntity, TKey> specification);
        Task<TEntity> GetByIdWithSpecAsync(ISpecification<TEntity, TKey> specification);

    }
}
