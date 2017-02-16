using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Enums;
using ServiceLayer.Repository;

namespace ServiceLayer.Service
{
    public class OrderService : EntityService, IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly Lazy<IOrderHistoryService> _orderHistoryService;
        private readonly Lazy<IRateService> _rateService;
        public OrderService(IEntityRepository entityRepository,
            IOrderRepository orderReoiRepository,
            IOrderHistoryService driverOrderService, 
            IRateService rateService) : base(entityRepository)
        {
            _orderRepository = orderReoiRepository;
            _rateService = new Lazy<IRateService>(() => rateService);
            _orderHistoryService = new Lazy<IOrderHistoryService>(() => driverOrderService);
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            await _orderRepository.CreateOrderAsync(order);
            var orderHistory = new OrderHistory
            {
                Action = ActionType.OrderCreated,
                CreatedBy = order.Business.ContactPerson.Id,
                CreatedDt = DateTime.UtcNow,
                IsDeleted = false,
                OrderId = order.Id,
                UpdatedBy = order.Business.ContactPerson.Id,
                UpdatedDt = DateTime.UtcNow,
                TimeToReachDropOffLocation = order.TimeToReachDropOffLocation,
                TimeToReachPickUpLocation = order.TimeToReachPickUpLocation,
                OrderPrice = order.OrderPrice
            };

            await _orderHistoryService.Value.CreateNewRecordAsync(orderHistory);
            return order;
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
                TimeToReachPickUpLocation = order.TimeToReachPickUpLocation,
                OrderPrice = order.OrderPrice
            };

            await _orderHistoryService.Value.CreateNewRecordAsync(orderHistory);
        }

        public async Task AcceptDriverForOrderAsync(int orderId, int driverId)
        {
            var driver = await _orderRepository.GetByIdAsync<Driver>(driverId);
            var order = await _orderRepository.GetByIdAsync<Order>(orderId);

            await _orderRepository.AcceptDriverForOrderAsync(driver, order);

            var orderHistory = new OrderHistory
            {
                Action = ActionType.DriverAcceptedOrder,
                CreatedBy = order.Business.ContactPerson.Id,
                CreatedDt = DateTime.UtcNow,
                DriverId = driver.Id,
                IsDeleted = false,
                OrderId = order.Id,
                UpdatedBy = order.Business.ContactPerson.Id,
                UpdatedDt = DateTime.UtcNow,
                TimeToReachDropOffLocation = order.TimeToReachDropOffLocation,
                TimeToReachPickUpLocation = order.TimeToReachPickUpLocation,
                OrderPrice = order.OrderPrice
            };

            await _orderHistoryService.Value.CreateNewRecordAsync(orderHistory);
        }

        public async Task<decimal> CalculateOrderPriceAsync(Order order, Driver driver)
        {
            // TODO: Check to see if there are any discounts for specified business

            var bookingFee = await _rateService.Value.GetPaymentByPaymentTypeAsync(PaymentType.OrderBookingFee);
            if (driver.HasAotmBox)
                bookingFee += await _rateService.Value.GetPaymentByPaymentTypeAsync(PaymentType.AotmBox);

            // TODO: calculate the mileage for the driver. Should we calculate mileage?

            return bookingFee;
        }

        public async Task UpdateOrderAsync(Order order, Person person)
        {
            order.UpdatedBy = person.Id;
            order.UpdatedDt = DateTime.UtcNow;
            await _orderRepository.CreateOrderAsync(order);
        }

        public async Task CancelDriverForOrderAsync(int orderId, int driverId)
        {
            var order = await _orderRepository.GetByIdAsync<Order>(orderId);

            await _orderRepository.CancelDriverForOrderAsync(order);

            var orderHistory = new OrderHistory
            {
                Action = ActionType.DriverCanceledByBusiness,
                CreatedBy = order.Business.ContactPerson.Id,
                CreatedDt = DateTime.UtcNow,
                DriverId = driverId,
                IsDeleted = false,
                OrderId = order.Id,
                UpdatedBy = order.Business.ContactPerson.Id,
                UpdatedDt = DateTime.UtcNow,
                TimeToReachDropOffLocation = order.TimeToReachDropOffLocation,
                TimeToReachPickUpLocation = order.TimeToReachPickUpLocation,
                OrderPrice = order.OrderPrice
            };

            await _orderHistoryService.Value.CreateNewRecordAsync(orderHistory);
        }

        public async Task RejectOrderAsync(Order order, Driver driver)
        {
            await _orderRepository.RejectOrderAsync(order, driver);

            var orderHistory = new OrderHistory
            {
                Action = ActionType.DriverRejectedOrder,
                CreatedBy = driver.Id,
                CreatedDt = DateTime.UtcNow,
                DriverId = driver.Id,
                IsDeleted = false,
                OrderId = order.Id,
                UpdatedBy = driver.Id,
                UpdatedDt = DateTime.UtcNow,
                TimeToReachDropOffLocation = order.TimeToReachDropOffLocation,
                TimeToReachPickUpLocation = order.TimeToReachPickUpLocation,
                OrderPrice = order.OrderPrice
            };

            await _orderHistoryService.Value.CreateNewRecordAsync(orderHistory);
        }

        public async Task OnTheWayToPickUpAsync(Driver driver, Order order)
        {
            await _orderRepository.OnTheWayToPickUpAsync(order, driver);

            var orderHistory = new OrderHistory
            {
                Action = ActionType.DriverIsOnTheWayToPickUp,
                CreatedBy = driver.Id,
                CreatedDt = DateTime.UtcNow,
                DriverId = driver.Id,
                IsDeleted = false,
                OrderId = order.Id,
                UpdatedBy = driver.Id,
                UpdatedDt = DateTime.UtcNow,
                TimeToReachDropOffLocation = order.TimeToReachDropOffLocation,
                TimeToReachPickUpLocation = order.TimeToReachPickUpLocation,
                OrderPrice = order.OrderPrice
            };

            await _orderHistoryService.Value.CreateNewRecordAsync(orderHistory);
        }

        public async Task ArrivedAtPickUpLocationAsync(Driver driver, Order order)
        {
            await _orderRepository.ArrivedAtPickUpLocationAsync(order, driver);

            var lastOrderRecord =
                await
                    _orderHistoryService.Value.GetRecordByDriverIdOrderIdAndActionTypeAsync(driver.Id, order.Id,
                        ActionType.DriverIsOnTheWayToPickUp);

            var actualTimeToReachPickUpLocation = (DateTime.UtcNow - lastOrderRecord.UpdatedDt).TotalMinutes;

            var orderHistory = new OrderHistory
            {
                Action = ActionType.DriverArrivedAtPickUpLocation,
                CreatedBy = driver.Id,
                CreatedDt = DateTime.UtcNow,
                DriverId = driver.Id,
                IsDeleted = false,
                OrderId = order.Id,
                UpdatedBy = driver.Id,
                UpdatedDt = DateTime.UtcNow,
                TimeToReachDropOffLocation = order.TimeToReachDropOffLocation,
                TimeToReachPickUpLocation = order.TimeToReachPickUpLocation,
                ActuallTimeToPickUpLocation = new decimal(actualTimeToReachPickUpLocation),
                OrderPrice = order.OrderPrice
            };

            await _orderHistoryService.Value.CreateNewRecordAsync(orderHistory);
        }

        public async Task OrderPickedUpAsync(Driver driver, Order order)
        {
            await _orderRepository.OrderPickedUpAsync(order, driver);

            var lastOrderRecord =
                await
                    _orderHistoryService.Value.GetRecordByDriverIdOrderIdAndActionTypeAsync(driver.Id, order.Id,
                        ActionType.DriverArrivedAtPickUpLocation);

            var orderHistory = new OrderHistory
            {
                Action = ActionType.DriverPickedUpTheOrder,
                CreatedBy = driver.Id,
                DriverId = driver.Id,
                CreatedDt = DateTime.UtcNow,
                IsDeleted = false,
                OrderId = order.Id,
                UpdatedBy = driver.Id,
                UpdatedDt = DateTime.UtcNow,
                TimeToReachDropOffLocation = order.TimeToReachDropOffLocation,
                TimeToReachPickUpLocation = order.TimeToReachPickUpLocation,
                ActuallTimeToPickUpLocation = lastOrderRecord?.ActuallTimeToPickUpLocation,
                OrderPrice = order.OrderPrice
            };

            await _orderHistoryService.Value.CreateNewRecordAsync(orderHistory);
        }

        public async Task OrderDeliveredAsync(Driver driver, Order order)
        {
            await _orderRepository.OrderDelivieredAsync(order, driver);

            var lastOrderRecord =
                await
                    _orderHistoryService.Value.GetRecordByDriverIdOrderIdAndActionTypeAsync(driver.Id, order.Id,
                        ActionType.DriverPickedUpTheOrder);

            var actualTimeToReachDropOffLocation = (DateTime.UtcNow - lastOrderRecord.UpdatedDt).TotalMinutes;

            var orderHistory = new OrderHistory
            {
                Action = ActionType.OrderDelivered,
                CreatedBy = driver.Id,
                DriverId = driver.Id,
                CreatedDt = DateTime.UtcNow,
                IsDeleted = false,
                OrderId = order.Id,
                UpdatedBy = driver.Id,
                UpdatedDt = DateTime.UtcNow,
                TimeToReachDropOffLocation = order.TimeToReachDropOffLocation,
                TimeToReachPickUpLocation = order.TimeToReachPickUpLocation,
                ActuallTimeToPickUpLocation = lastOrderRecord.ActuallTimeToPickUpLocation,
                ActualTimeToDropOffLocation = new decimal(actualTimeToReachDropOffLocation),
                OrderPrice = order.OrderPrice
            };

            await _orderHistoryService.Value.CreateNewRecordAsync(orderHistory);
        }

        public async Task OrderNotDeliveredAsync(Driver driver, Order order, string reason)
        {
            await _orderRepository.OrderNotDeliveredAsync(order, driver, reason);
            var lastOrderRecord =
                await
                    _orderHistoryService.Value.GetRecordByDriverIdOrderIdAndActionTypeAsync(driver.Id, order.Id,
                        ActionType.DriverPickedUpTheOrder);

            var orderHistory = new OrderHistory
            {
                Action = ActionType.OrderNotDelivered,
                CreatedBy = driver.Id,
                CreatedDt = DateTime.UtcNow,
                IsDeleted = false,
                OrderId = order.Id,
                UpdatedBy = driver.Id,
                UpdatedDt = DateTime.UtcNow,
                TimeToReachDropOffLocation = order.TimeToReachDropOffLocation,
                TimeToReachPickUpLocation = order.TimeToReachPickUpLocation,
                ActuallTimeToPickUpLocation = lastOrderRecord.ActuallTimeToPickUpLocation,
                OrderPrice = order.OrderPrice,
                NotDeliveredReason = reason
            };

            await _orderHistoryService.Value.CreateNewRecordAsync(orderHistory);
        }

        public async Task<IEnumerable<Order>> GetBusinessOrdersAsync(int businessId, OrderStatus? status)
        {
            return await _orderRepository.GetBusinessOrdersAsync(businessId, status);
        }
    }
}