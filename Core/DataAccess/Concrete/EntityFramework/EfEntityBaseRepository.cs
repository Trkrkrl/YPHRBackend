using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Core.DataAccess.Abstract;

namespace Core.DataAccess.Concrete.EntityFramework
{
    public class EfEntityBaseRepository<TEntity, TContext> : IBaseRepository<TEntity>
        where TEntity : class, IEntity, new()
        where TContext : DbContext, new()
    {
        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await using var context = new TContext();
            context.Entry(entity).State = EntityState.Added;
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> AnyAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            await using var context = new TContext();
            return await context.Set<TEntity>().AnyAsync(predicate, cancellationToken);
        }

        public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await using var context = new TContext();
            context.Entry(entity).State = EntityState.Deleted;
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<TEntity?> GetAsync(
            Expression<Func<TEntity, bool>> filter,
            CancellationToken cancellationToken = default)
        {

            await using var context = new TContext();
            return await context.Set<TEntity>().FirstOrDefaultAsync(filter, cancellationToken);
        }

        public async Task<TEntity?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            await using var context = new TContext();

            return await context.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<List<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            int page = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            await using var context = new TContext();
            IQueryable<TEntity> query = context.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            var skip = (page - 1) * pageSize;

            return await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await using var context = new TContext();
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
