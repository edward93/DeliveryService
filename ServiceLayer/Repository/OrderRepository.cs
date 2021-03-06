﻿using System;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using DAL.Context;
using DAL.Entities;
using DAL.Enums;

namespace ServiceLayer.Repository
{
    public class OrderRepository : EntityRepository, IOrderRepository
    {
        public OrderRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            DbContext.Orders.AddOrUpdate(order);
            await DbContext.SaveChangesAsync();
            return order;
        }

        /// <summary>
        /// After business accepted driver for this order now driver has to accept the order.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="driver"></param>
        /// <returns></returns>
        public async Task AcceptOrderAsync(Order order, Driver driver)
        {
            order.AssignedDriverId = driver.Id;

            await ChangeOrderStatus(order, OrderStatus.AcceptedByDriver, driver.Person);
        }

        /// <summary>
        /// When order created and program automatically finds closest driver, business has to accept the driver found by 
        /// system befor driver could actually see that he/she has a new order.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public async Task AcceptDriverForOrderAsync(Driver driver, Order order)
        {
            await ChangeOrderStatus(order, OrderStatus.DriverAcceptedByBusiness, driver.Person);
        }

        public async Task CancelDriverForOrderAsync(Order order)
        {
            await ChangeOrderStatus(order, OrderStatus.Pending, order.Business.ContactPerson);
        }

        public async Task RejectOrderAsync(Order order, Driver driver)
        {
            await ChangeOrderStatus(order, OrderStatus.RejectedByDriver, driver.Person);

        }

        public async Task OnTheWayToPickUpAsync(Order order, Driver driver)
        {
            await ChangeOrderStatus(order, OrderStatus.OnTheWayToPickUp, driver.Person);
        }

        public async Task ChangeOrderStatus(Order order, OrderStatus newStatus, Person person)
        {
            order.OrderStatus = newStatus;
            order.UpdatedDt = DateTime.UtcNow;
            order.UpdatedBy = person.Id;

            DbContext.Orders.AddOrUpdate(order);
            await DbContext.SaveChangesAsync();
        }

        public async Task ArrivedAtPickUpLocationAsync(Order order, Driver driver)
        {
            await ChangeOrderStatus(order, OrderStatus.ArrivedAtThePickUpLocation, driver.Person);
        }

        public async Task OrderPickedUpAsync(Order order, Driver driver)
        {
            await ChangeOrderStatus(order, OrderStatus.OrderPickedUp, driver.Person);
        }

        public async Task OrderDelivieredAsync(Order order, Driver driver)
        {
            await ChangeOrderStatus(order, OrderStatus.Delivered, driver.Person);
        }

        public async Task OrderNotDeliveredAsync(Order order, Driver driver, string reason)
        {
            order.NotDeliveredReason = reason;

            await ChangeOrderStatus(order, OrderStatus.Delivered, driver.Person);

        }
    }
}