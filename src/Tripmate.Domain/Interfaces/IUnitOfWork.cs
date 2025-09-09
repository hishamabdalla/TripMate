using Tripmate.Domain.Entities.Base;
using Tripmate.Domain.Interfaces.Repositories.Intefaces;

namespace Tripmate.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// 
        /// Gets a repository for the specified entity type and key type.
        ///     
        /// </summary>


        IGenericRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : BaseEntity<TKey>;
        Task SaveChangesAsync();
    }
}
