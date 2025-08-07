using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Entities.Base;
using Tripmate.Domain.Interfaces.Repositories.Intefaces;
using Tripmate.Infrastructure.Data.Context;

namespace Tripmate.Infrastructure.Repositories
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        private readonly TripmateDbContext _context;
        private readonly DbSet<TEntity> _dbSet;
        public GenericRepository(TripmateDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }
        public async Task AddAsync(TEntity entity) => await _dbSet.AddAsync(entity);

        public async Task<IEnumerable<TEntity>> GetAllAsync()=> await _dbSet.ToListAsync();

        public async Task<TEntity> GetByIdAsync(TKey id)=> await _dbSet.FindAsync(id);

        public void Update(TEntity entity)
        {
           
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;

        }
        public void Delete(TKey id)
        {
            var entity = _dbSet.Find(id);

            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

    }
}
