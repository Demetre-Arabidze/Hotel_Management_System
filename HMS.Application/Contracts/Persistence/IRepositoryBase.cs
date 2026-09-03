using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HMS.Application.Contracts.Persistence
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<(IEnumerable<T> Items, int TotalCount)> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            int? pageNumber = null,
            int? pageSize = null,
            Expression<Func<T, object>>? orderBy = null,
            bool ascending = true,
            CancellationToken cancellationToken = default,
            bool tracking = false,
            params Expression<Func<T, object>>[] includes);

        Task<T?> GetAsync(
            Expression<Func<T, bool>> filter,
            bool tracking = true,
            CancellationToken cancellationToken = default,
            Func<IQueryable<T>, IQueryable<T>>? include = null);

        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task<int> SaveAsync(CancellationToken cancellationToken = default);
        void Remove(T entity);
        void Update(T entity);
        void RemoveRange(IEnumerable<T> entities);

        Task<bool> ExistsAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);
    }
}
