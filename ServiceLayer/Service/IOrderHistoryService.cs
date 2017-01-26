using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Enums;

namespace ServiceLayer.Service
{
    public interface IOrderHistoryService : IEntityService
    {
        Task<OrderHistory> CreateNewRecordAsync(OrderHistory record);
        Task<IEnumerable<OrderHistory>> GetRejectedOrdersByDriverForCurrentDayAsync(int driverId);
        Task<OrderHistory> GetRecordByDriverIdOrderIdAndActionTypeAsync(int driverId, int orderId, ActionType actionType);
    }
}