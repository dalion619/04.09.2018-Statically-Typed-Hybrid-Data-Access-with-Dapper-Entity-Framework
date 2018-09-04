using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Example.DynamicMapping.EntityModels;

namespace Example.DynamicMapping.DataAccess.EF
{
    public class EntityFrameworkRepository<TContext> : IDisposable where TContext : DbContext
    {
        protected readonly TContext context;

        public EntityFrameworkRepository(TContext context)
        {
            this.context = context;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.context.Dispose();
            }
            // free native resources if there are any.
        }

        protected virtual IQueryable<TEntity> GetQueryable<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
            where TEntity : class, IEntityBase
        {
            includeProperties = includeProperties ?? string.Empty;
            IQueryable<TEntity> query = context.Set<TEntity>();

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

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return query;
        }



        public virtual async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
            where TEntity : class, IEntityBase
        {
            return await GetQueryable<TEntity>(null, orderBy, includeProperties, skip, take).ToListAsync();
        }


        public virtual async Task<IEnumerable<TEntity>> GetAsync<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
            where TEntity : class, IEntityBase
        {
            return await GetQueryable<TEntity>(filter, orderBy, includeProperties, skip, take).ToListAsync();
        }


        public virtual async Task<TEntity> GetOneAsync<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = null)
            where TEntity : class, IEntityBase
        {
            return await GetQueryable<TEntity>(filter, null, includeProperties).SingleOrDefaultAsync();
        }



        public virtual async Task<TEntity> GetFirstAsync<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null)
            where TEntity : class, IEntityBase
        {
            return await GetQueryable<TEntity>(filter, orderBy, includeProperties).FirstOrDefaultAsync();
        }


        public virtual Task<TEntity> GetByIdAsync<TEntity>(object id)
            where TEntity : class, IEntityBase
        {
            return context.Set<TEntity>().FindAsync(id);
        }


        public virtual Task<int> GetCountAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class, IEntityBase
        {
            return GetQueryable<TEntity>(filter).CountAsync();
        }


        public virtual Task<bool> GetExistsAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class, IEntityBase
        {
            return GetQueryable<TEntity>(filter).AnyAsync();
        }

    }
}
