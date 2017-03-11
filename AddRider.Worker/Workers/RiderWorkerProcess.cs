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
        public RiderWorkerProcess(IRiderService riderService)
        {
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
    }
}