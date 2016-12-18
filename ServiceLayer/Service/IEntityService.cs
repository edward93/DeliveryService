using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Service
{
    public interface IEntityService
    {
        Task<T> GetByIdAsync<T>(int entityId) where T : Entity;
        Task<IEnumerable<T>> GetAllEntitiesAsync<T>() where T : Entity;
    }
}