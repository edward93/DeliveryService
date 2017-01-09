using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Repository
{
    public interface IEntityRepository
    {
        Task<T> GetByIdAsync<T>(int entityId) where T : class, IEntity;
        Task<IEnumerable<T>> GetAllEntitiesAsync<T>() where T : class, IEntity;
        Task<T> RemoveEntityAsync<T>(int entityId) where T : class, IEntity;
    }
}