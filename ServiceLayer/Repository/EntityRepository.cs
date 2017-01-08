using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DAL.Context;
using DAL.Entities;

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

        public async Task<T> GetByIdAsync<T>(int entityId) where T : class, IEntity
        {
            return await DbContext.Set<T>().FirstOrDefaultAsync(c => c.Id == entityId && c.IsDeleted == false);
        }
    }
}