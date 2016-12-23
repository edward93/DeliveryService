﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
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
        public async Task<IEnumerable<T>> GetAllEntitiesAsync<T>() where T : Entity
        {
            return await DbContext.Set<T>().Where(c => c.IsDeleted == false).ToListAsync();
        }

        public async Task<T> RemoveEntity<T>(int entityId) where T : Entity
        {
            var entityToRemove = await GetByIdAsync<T>(entityId);
            entityToRemove.IsDeleted = true;
            DbContext.Set<T>().AddOrUpdate(entityToRemove);
            await DbContext.SaveChangesAsync();
            return entityToRemove;
        }

        public async Task<T> GetByIdAsync<T>(int entityId) where T : Entity
        {
            return await DbContext.Set<T>().FirstOrDefaultAsync(c => c.Id == entityId && c.IsDeleted == false);
        }
    }
}