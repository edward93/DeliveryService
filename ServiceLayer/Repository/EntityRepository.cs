using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DAL.Context;
using DAL.Entities;
using DAL.Entities.Interfaces;

namespace ServiceLayer.Repository
{
    public class EntityRepository : IEntityRepository
    {
        protected readonly IDbContext DbContext;

        public EntityRepository(IDbContext dbContext)
        {
            DbContext = dbContext;
        }
        public async Task<IEnumerable<T>> GetAllEntitiesAsync<T>() where T : class, IEntity
        {
            return await DbContext.Set<T>().Where(c => c.IsDeleted == false).ToListAsync();
        }

        public async Task<T> RemoveEntityAsync<T>(int entityId) where T : class, IEntity
        {
            var entityToRemove = await GetByIdAsync<T>(entityId);
            entityToRemove.IsDeleted = true;
            DbContext.Set<T>().AddOrUpdate(entityToRemove);
            await DbContext.SaveChangesAsync();
            return entityToRemove;
        }

        public async Task<T> CreateEntityAsync<T>(T entity) where T : class, IEntity
        {
            DbContext.Set<T>().AddOrUpdate(entity);
            await DbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<T>> GetAllEntitiesAsync<T>(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includes) where T : class, IEntity
        {
            IQueryable<T> query = DbContext.Set<T>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includes != null)
            {
                query = includes.Aggregate(query,
                          (current, include) => current.Include(include));
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }

            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync<T>(int entityId) where T : class, IEntity
        {
            return await DbContext.Set<T>().FirstOrDefaultAsync(c => c.Id == entityId && c.IsDeleted == false);
        }
    }
}