using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DAL.Context;
using DAL.Entities;
using DAL.Entities.Interfaces;
using ServiceLayer.Repository;

namespace ServiceLayer.Service
{
    public class EntityService : IEntityService
    {
        private readonly IEntityRepository _entityRepository;
        internal DbSet<IEntity> DbSet;

        public EntityService(IEntityRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }
        public async Task<T> GetByIdAsync<T>(int entityId) where T : class, IEntity
        {
            return await _entityRepository.GetByIdAsync<T>(entityId);
        }

        public async Task<IEnumerable<T>> GetAllEntitiesAsync<T>() where T : class, IEntity
        {
            return await _entityRepository.GetAllEntitiesAsync<T>();
        }

        public async Task<T> RemoveEntityAsync<T>(int entityId) where T : class, IEntity
        {
            return await _entityRepository.RemoveEntityAsync<T>(entityId);
        }

        public IEnumerable<T> Get<T>(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includes) where T : class, IEntity
        {
            IDbContext context = new DAL.Context.DbContext();
            DbSet<T> dbSet = context.Set<T>();
            IQueryable<T> query = dbSet;

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
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }
    }
}