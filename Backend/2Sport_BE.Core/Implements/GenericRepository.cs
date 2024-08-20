using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace _2Sport_BE.Repository.Implements
{
    internal class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly TwoSportDBContext _dbContext;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(TwoSportDBContext context)
        {
            _dbContext = context;
            _dbSet = _dbContext.Set<T>();
        }
        public Task<int> CountAsync(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query.CountAsync();
        }

        public async Task DeleteAsync(int id)
        {
            T entityToDelete = await _dbSet.FindAsync(id);
            if (entityToDelete != null)
            {
                await DeleteAsync(entityToDelete);
            }
        }

        public async Task DeleteAsync(T entityToDelete)
        {
            if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
        }

        public async Task<T> FindAsync(int? id)
        {
            return await _dbSet.FindAsync(id);
        }


        public IQueryable<T> GetAll()
        {
            IQueryable<T> result = this._dbSet;
            return result;
        }

        public async Task<IEnumerable<T>> GetAllAsync(params string[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter = null)
        {

            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Using AsNoTracking for read-only queries
            return await query.AsNoTracking().ToListAsync();

        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter = null,
                                string includeProperties = "",
                                int? pageIndex = null,
                                int? pageSize = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize = pageSize.Value > 0 ? pageSize.Value : 10;

                query = query.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter = null,
                                Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                string includeProperties = "",
                                int? pageIndex = null,
                                int? pageSize = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize = pageSize.Value > 0 ? pageSize.Value : 10;

                query = query.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            return await query.ToListAsync();
        }

        public async Task<T> GetByIDAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> GetObjectAsync(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _dbSet;
            return await query.Where(filter).FirstOrDefaultAsync();
        }

        public async Task InsertAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task InsertRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _dbContext.Entry(entityToUpdate).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();
        }
    }
}
