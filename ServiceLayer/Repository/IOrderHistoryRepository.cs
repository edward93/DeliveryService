using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Repository
{
    public interface IOrderHistoryRepository :IEntityRepository
    {
        Task<OrderHistory> CreateNewRecordAsync(OrderHistory record);
        Task<IEnumerable<OrderHistory>> GetRejectedOrdersByDriverForCurrentDayAsync(int driverId);
    }
}