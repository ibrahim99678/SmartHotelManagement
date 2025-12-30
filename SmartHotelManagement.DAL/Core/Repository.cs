using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SmartHotelManagement.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SmartHotelManagement.DAL.Core
{
    public class Repository<TEntity, Tkey, TContext> : IRepository<TEntity, Tkey, TContext> 
                where TEntity : class
                where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly DbSet<TEntity> _dbSet;
        public Repository(TContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public virtual async Task<IList<TResult>> GetAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderby = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            bool disableTracking = true, string includProperties = "")
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable();

            if(include != null)
                query = include(query);
            if(predicate != null)
                query.Where(predicate);
            if(orderby != null)
                query=orderby(query);
            if(disableTracking)
                query = query.AsNoTracking();

            var result = await query.Select(selector).ToListAsync();
            return result;
        }
        public virtual async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual async Task<TResult> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderby = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            bool disableTracking = true, string includProperties = "")
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable();

            if (include != null)
                query = include(query);
            if (predicate != null)
                query.Where(predicate);
            if (orderby != null)
                query = orderby(query);
            if (disableTracking)
                query = query.AsNoTracking();

            var result = await query.Select(selector).FirstOrDefaultAsync();
            return result;
        }

        
        public async Task<TEntity> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var query = _dbSet.AsQueryable();
            return await query.AnyAsync(predicate);
        }

        public virtual async Task UpdateAsync(TEntity entity, params string[] updateProperties)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;

            if (updateProperties != null && updateProperties.Any())
                UpdateProperty(updateProperties);
        }

        private void UpdateProperty(params string[] updateProperties)
        {
            var modifiesEntries = _context.ChangeTracker.Entries().Where(e=>e.State == EntityState.Modified).ToList();
            if (updateProperties.Any())
            {
                foreach (var entity in modifiesEntries)
                {
                    entity.Properties.ToList().ForEach(y=> y.IsModified= false);
                    foreach (var property in updateProperties)
                    {
                        entity.Property(property).IsModified=typeof(TEntity).GetProperty(property)!=null;
                    }
                }
            }
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            if (_context.Entry(entity).State == EntityState.Deleted)
            {
                _dbSet.Attach(entity);

            }
            _dbSet.Remove(entity);
        }


    }
}
