using System.Linq.Expressions;

namespace _2Sport_BE.Repository.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> FindAsync(int? id);
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter = null);
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter = null,
                                  string includeProperties = "",
                                  int? pageIndex = null,
                                  int? pageSize = null);
        Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "",
            int? pageIndex = null,
            int? pageSize = null);
        Task<T> GetObjectAsync(Expression<Func<T, bool>> filter = null);
        Task<int> CountAsync(Expression<Func<T, bool>> filter = null);

        Task<IEnumerable<T>> GetAllAsync(params string[] includes);


        Task InsertAsync(T entity);
        Task InsertRangeAsync(IEnumerable<T> entities);

        Task DeleteAsync(int id);

        Task DeleteAsync(T entityToDelete);

        Task UpdateAsync(T entityToUpdate);
    }
}
