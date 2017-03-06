using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Enums;
using ServiceLayer.Repository;

namespace ServiceLayer.Service
{
    public class OrderHistoryService : EntityService, IOrderHistoryService
    {
        private readonly IOrderHistoryRepository _repository;
        public OrderHistoryService(IEntityRepository entityRepository,
            IOrderHistoryRepository repository) : base(entityRepository)
        {
            _repository = repository;
        }

        public async Task<OrderHistory> CreateNewRecordAsync(OrderHistory record)
        {
            return await _repository.CreateNewRecordAsync(record);
        }

        public async Task<IEnumerable<OrderHistory>> GetRejectedOrdersByDriverForCurrentDayAsync(int driverId)
        {
            return await _repository.GetRejectedOrdersByDriverForCurrentDayAsync(driverId);
        }

        public async Task<OrderHistory> GetRecordByDriverIdOrderIdAndActionTypeAsync(int driverId, int orderId, ActionType actionType)
        {
            return await _repository.GetRecordByDriverIdOrderIdAndActionTypeAsync(driverId, orderId, actionType);
        }

        public async Task<IEnumerable<int>> GetDriverIdsWhoRejectedOrderOrGotRejectedByBusinessAsync(int orderId)
        {
            return await _repository.GetDriverIdsWhoRejectedOrderOrGotRejectedByBusinessAsync(orderId);
        }
    }
}