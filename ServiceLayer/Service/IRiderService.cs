using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Enums;
using DeliveryService.Models.ViewModels;

namespace ServiceLayer.Service
{
    public interface IRiderService : IEntityService
    {
        Task<Driver> CreateDriverAsync(Driver driver);
        Task<Driver> UpdateDriverAsync(Driver driver);
        Task<Driver> GetDriverByPersonAsync(string personId);
        Task<bool> DeleteDriver(int driverId);
        Task ChangeDriverStatusAsync(int driverId, RiderStatus newStatus);
        Task ApproveDriverAsync(int driverId, int currentPersonId);
        Task RejectDriverAsync(int driverId, int currentPersonId);
        Task<int> GetOnlineDriversCountAsync();
        Task<IEnumerable<DriverDetailsWithLocation>> GetOnlineDriversAsync();
        Task<IEnumerable<DriverDetailsWithLocation>> GetBusinessDriversAsync(int businessId);
        Task<IEnumerable<Driver>> GetRidersByStatusAsync(RiderStatus disconnectedFromHub);
        Task UpdateRidersWithDisconnectedFromHubStatusAsync();
    }
}
