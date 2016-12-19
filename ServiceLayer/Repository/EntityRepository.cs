using System.Collections.Generic;
using System.Data.Entity;
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
            return await DbContext.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync<T>(int entityId) where T : Entity
        {
            return await DbContext.Set<T>().FirstOrDefaultAsync(c => c.Id == entityId);
        }
    }
}