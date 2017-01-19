using System;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Enums;
using ServiceLayer.Repository;

namespace ServiceLayer.Service
{
    public class OrderService : EntityService, IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly Lazy<IOrderHistoryService> _driverOrderSerivce;
        public OrderService(IEntityRepository entityRepository,
            IOrderRepository orderReoiRepository,
            IOrderHistoryService driverOrderService) : base(entityRepository)
        {
            _orderRepository = orderReoiRepository;
            _driverOrderSerivce = new Lazy<IOrderHistoryService>(() => driverOrderService);
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            return await _orderRepository.CreateOrderAsync(order);
        }

        public async Task AcceptOrderAsync(Order order, Driver driver)
        {
            await _orderRepository.AcceptOrderAsync(order, driver);

            var orderHistory = new OrderHistory
            {
                Action = ActionType.DriverAcceptedOrder,
                CreatedBy = driver.Id,
                CreatedDt = DateTime.UtcNow,
                DriverId = driver.Id,
                IsDeleted = false,
                OrderId = order.Id,
                UpdatedBy = driver.Id,
                UpdatedDt = DateTime.UtcNow,
                TimeToReachDropOffLocation = order.TimeToReachDropOffLocation,
                TimeToReachPickUpLocation = order.TimeToReachPickUpLocation
            };

            await _driverOrderSerivce.Value.CreateNewRecordAsync(orderHistory);
        }
    }
}