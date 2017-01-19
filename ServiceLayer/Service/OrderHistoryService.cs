using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities;
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
    }
}