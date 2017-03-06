using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Enums;

namespace ServiceLayer.Repository
{
    public interface IOrderHistoryRepository :IEntityRepository
    {
        Task<OrderHistory> CreateNewRecordAsync(OrderHistory record);
        Task<IEnumerable<OrderHistory>> GetRejectedOrdersByDriverForCurrentDayAsync(int driverId);
        Task<OrderHistory> GetRecordByDriverIdOrderIdAndActionTypeAsync(int driverId, int orderId, ActionType actionType);
        Task<IEnumerable<int>> GetDriverIdsWhoRejectedOrderOrGotRejectedByBusinessAsync(int orderId);
    }
}