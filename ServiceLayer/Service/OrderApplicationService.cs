using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Service
{
    public class OrderApplicationService : IOrderApplicationService
    {
        private readonly Lazy<IRiderService> _riderService;
        private readonly Lazy<IOrderService> _orderService;
        private readonly Lazy<IOrderHistoryService> _orderHistoryService;
        private readonly Lazy<IRiderPenaltyService> _riderPenaltyService;
        public OrderApplicationService(IRiderService riderService, 
            IOrderService orderService, 
            IOrderHistoryService orderHistoryService, 
            IRiderPenaltyService riderPenaltyService)
        {
            _riderPenaltyService = new Lazy<IRiderPenaltyService>(() => riderPenaltyService);
            _riderService = new Lazy<IRiderService>(() => riderService);
            _orderService = new Lazy<IOrderService>(() => orderService);
            _orderHistoryService = new Lazy<IOrderHistoryService>(() => orderHistoryService);
        }
        public async Task RejectOrderAsync(int orderId, int riderId)
        {
            var order = await _orderService.Value.GetByIdAsync<Order>(orderId);
            if (order == null) throw new Exception("No order found for given id");

            var rider = await _riderService.Value.GetByIdAsync<Driver>(riderId);
            if (rider == null) throw new Exception("No rider was found for given id");

            await _orderService.Value.RejectOrderAsync(order, rider);

            // Calculate rejection penalty if ther is one
            var orders = await _orderHistoryService.Value.GetRejectedOrdersByDriverForCurrentDayAsync(riderId);

            // TODO: test this method
            if (orders.Count() >= 3)
            {
                // Penalize driver for rejecting more then 3 times during last 24 hours.
                await
                    _riderPenaltyService.Value.PenalizeDriverForRejectingMoreThenThreeTimesAsync(rider, order);
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersThatShouldBeRejectedOnBehalfOfRider(int timeInSeconds)
        {
            return await _orderService.Value.GetOrdersThatShouldBeRejectedOnBehalfOfRider(timeInSeconds);
        }
    }
}