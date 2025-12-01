using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.DataAccess.Abstract
{
    public interface IBaseRepository<T> where T : class, IEntity, new()
    {
        Task<List<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null, int page=1, int pageSize=10,

            CancellationToken cancellationToken = default);

        Task<T?> GetAsync(
            Expression<Func<T, bool>> filter,
            CancellationToken cancellationToken = default);

        Task<T?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    }
}
