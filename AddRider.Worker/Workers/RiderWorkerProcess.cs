using System;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Enums;
using ServiceLayer.Service;

namespace AddRider.Worker.Workers
{
    public class RiderWorkerProcess
    {
        private readonly Lazy<IRiderService> _riderService;
        private readonly Lazy<IOrderApplicationService> _orderApplicationService;
        public RiderWorkerProcess(IRiderService riderService, 
            IOrderApplicationService orderService)
        {
            _orderApplicationService = new Lazy<IOrderApplicationService>(() => orderService);
            _riderService = new Lazy<IRiderService>(() => riderService);
        }

        /// <summary>
        /// This method changes rider status from disconnected from hub to offline 
        /// </summary>
        /// <returns></returns>
        public async Task RiderStatusProcessing()
        {
            Console.WriteLine($"{nameof(RiderStatusProcessing)} has started - {DateTime.UtcNow}");

            // Update rider statuses who have disconnected from hub stats
            await _riderService.Value.UpdateRidersWithDisconnectedFromHubStatusAsync();

            Console.WriteLine($"{nameof(RiderStatusProcessing)} has finished - {DateTime.UtcNow}");
        }

        /// <summary>
        /// Find and reject all orders that have status accepted by business and are updated more than 60 sec ago 
        /// </summary>
        /// <returns></returns>
        public async Task RejectOrderAsync()
        {
            Console.WriteLine($"{nameof(RejectOrderAsync)} has started - {DateTime.UtcNow}");

            var timeInSeconds = 60;
            var orders = await _orderApplicationService.Value.GetOrdersThatShouldBeRejectedOnBehalfOfRider(timeInSeconds);

            foreach (var order in orders)
            {
                if (order.AssignedDriverId != null)
                    await _orderApplicationService.Value.RejectOrderAsync(order.Id, order.AssignedDriverId.Value);
                else throw new Exception("Something went horribly wrong");
            }

            Console.WriteLine($"{nameof(RejectOrderAsync)} has finished - {DateTime.UtcNow}");
        }
    }
}