using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Repository
{
    public interface IEntityRepository
    {
        Task<T> GetByIdAsync<T>(int entityId) where T : Entity;
        Task<IEnumerable<T>> GetAllEntitiesAsync<T>() where T : Entity;
    }
}