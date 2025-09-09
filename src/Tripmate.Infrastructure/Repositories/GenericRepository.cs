using Microsoft.EntityFrameworkCore;
using Tripmate.Domain.Entities.Base;
using Tripmate.Domain.Interfaces.Repositories.Intefaces;
using Tripmate.Domain.Specification;
using Tripmate.Infrastructure.Data.Context;
using Tripmate.Infrastructure.Specification;

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

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
           

            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(TKey id)=> await _dbSet.FindAsync(id);

        public void Update(TEntity entity)
        {
           
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;

        }
        public void Delete(TEntity entity)
        {
            

            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        // Specification methods
        public async Task<IEnumerable<TEntity>> GetAllWithSpecAsync(ISpecification<TEntity, TKey> specification)
        {
            return await ApplySpecification(specification).ToListAsync();
        }
        public async Task<TEntity> GetByIdWithSpecAsync( ISpecification<TEntity, TKey> specification)
        {
           return await ApplySpecification(specification)
                .FirstOrDefaultAsync();
        }

        private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity,TKey> specification)
        {
            return SpecificationEvaluator<TEntity, TKey>.GetQuery(_dbSet.AsQueryable(), specification);
        }
        public async Task<int> CountAsync(ISpecification<TEntity, TKey> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }

    }
}
