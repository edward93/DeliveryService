﻿using System;
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
    }
}