using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Entities.Interfaces;

namespace ServiceLayer.Service
{
    public interface IEntityService
    {
        Task<T> GetByIdAsync<T>(int entityId) where T : class, IEntity;
        Task<IEnumerable<T>> GetAllEntitiesAsync<T>() where T : class, IEntity;
        Task<T> RemoveEntityAsync<T>(int entityId) where T : class, IEntity;

        Task<IEnumerable<T>> GetAllEntitiesAsync<T>(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includes) where T : class, IEntity;
    }
}