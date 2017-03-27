using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Enums;
using DeliveryService.Models.ViewModels;
using ServiceLayer.Repository;

namespace ServiceLayer.Service
{
    public class RiderService : EntityService, IRiderService
    {
        private readonly IRiderRepository _riderRepository;
        public RiderService(IEntityRepository entityRepository,IRiderRepository repository) 
            : base(entityRepository)
        {
            _riderRepository = repository;
        }

        public async Task<Driver> CreateDriverAsync(Driver driver)
        {
            return await _riderRepository.CreateDriverAsync(driver);
        }

        public async Task<Driver> UpdateDriverAsync(Driver driver)
        {
            return await _riderRepository.CreateDriverAsync(driver);
        }
        public async Task<Driver> GetDriverByPersonAsync(string personId)
        {
            return await _riderRepository.GetDriverByPersonId(personId);
        }
        public async Task<bool> DeleteDriver(int driverId)
        {
            var driver = await _riderRepository.GetByIdAsync<Driver>(driverId);
            if (driver != null)
            {
                driver.IsDeleted = true;
                await UpdateDriverAsync(driver);
                return true;
            }
            return false;
        }

        public async Task ChangeDriverStatusAsync(int driverId, RiderStatus newStatus)
        {
            var driver = await GetByIdAsync<Driver>(driverId);

             if (driver == null) throw new Exception($"No driver found for given Id {driverId}");

            if (!driver.Approved) throw new Exception($"This driver (Id: {driverId}) is not approved and is not allowed to proceed");

            if (driver.Status == newStatus)
            {
                // this means that something is wrong since you cannot assign same status.
                throw new Exception($"Your status is already {driver.Status}");
            }

            driver.Status = newStatus;
            driver.UpdatedDt = DateTime.UtcNow;
            driver.UpdatedBy = driver.Person.Id;

            await UpdateDriverAsync(driver);
        }

        public async Task ApproveDriverAsync(int driverId, int currentPersonId)
        {
            await ChangeDriverStateAsync(driverId, true, currentPersonId);

        }

        public async Task RejectDriverAsync(int driverId, int currentPersonId)
        {
            await ChangeDriverStateAsync(driverId, false, currentPersonId);
        }

        public async Task<int> GetOnlineDriversCountAsync()
        {
            return await _riderRepository.GetOnlineDriversCountAsync();
        }

        public async Task<IEnumerable<DriverDetailsWithLocation>> GetOnlineDriversAsync()
        {
            return await _riderRepository.GetOnlineDriversAsync();
        }

        public async Task<IEnumerable<DriverDetailsWithLocation>> GetBusinessDriversAsync(int businessId)
        {
            return await _riderRepository.GetBusinessDriversAsync(businessId);
        }

        public async Task<IEnumerable<Driver>> GetRidersByStatusAsync(RiderStatus status)
        {
            return await _riderRepository.GetRidersByStatusAsync(status);
        }

        public async Task UpdateRidersWithDisconnectedFromHubStatusAsync()
        {
            await _riderRepository.UpdateRidersWithDisconnectedFromHubStatusAsync();
        }

        private async Task ChangeDriverStateAsync(int driverId, bool approved, int currentPersonId)
        {
            var driver = await GetByIdAsync<Driver>(driverId);
            if (driver == null) throw new Exception($"No driver found for given Id {driverId}");

            driver.Approved = approved;
            driver.UpdatedDt = DateTime.UtcNow;
            driver.UpdatedBy = currentPersonId;

            await UpdateDriverAsync(driver);
        }
    }
}
