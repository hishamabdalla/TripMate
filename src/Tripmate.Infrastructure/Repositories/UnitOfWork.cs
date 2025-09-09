using System.Collections;
using Tripmate.Domain.Entities.Base;
using Tripmate.Domain.Interfaces;
using Tripmate.Domain.Interfaces.Repositories.Intefaces;
using Tripmate.Infrastructure.Data.Context;

namespace Tripmate.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TripmateDbContext _context;
        private readonly Hashtable _repositories = new Hashtable();
        public UnitOfWork(TripmateDbContext context)
        {
            _context = context;
            _repositories = new Hashtable();
        }

        public IGenericRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {
           
            var type = typeof(TEntity).Name;
            if (!_repositories.ContainsKey(type))
            {
                var repository = new GenericRepository<TEntity, TKey>(_context);
                _repositories.Add(type, repository);
            }
            return (IGenericRepository<TEntity, TKey>)_repositories[type];

        }

        public async Task SaveChangesAsync()
        {
             await _context.SaveChangesAsync();
        }
    }
}
