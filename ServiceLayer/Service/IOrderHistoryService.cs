using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Service
{
    public interface IOrderHistoryService : IEntityService
    {
        Task<OrderHistory> CreateNewRecordAsync(OrderHistory record);
        Task<IEnumerable<OrderHistory>> GetRejectedOrdersByDriverForCurrentDayAsync(int driverId);
    }
}