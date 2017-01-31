using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Enums;
using ServiceLayer.Repository;

namespace ServiceLayer.Service
{
    public class DriverService : EntityService, IDriverService
    {
        private readonly IDriverRepository _driverRepository;
        public DriverService(IEntityRepository entityRepository,IDriverRepository repository) 
            : base(entityRepository)
        {
            _driverRepository = repository;
        }

        public async Task<Driver> CreateDriverAsync(Driver driver)
        {
            return await _driverRepository.CreateDriverAsync(driver);
        }

        public async Task<Driver> UpdateDriverAsync(Driver driver)
        {
            return await _driverRepository.CreateDriverAsync(driver);
        }
        public async Task<Driver> GetDriverByPersonAsync(string personId)
        {
            return await _driverRepository.GetDriverByPersonId(personId);
        }
        public async Task<bool> DeleteDriver(int driverId)
        {
            var driver = await _driverRepository.GetByIdAsync<Driver>(driverId);
            if (driver != null)
            {
                driver.IsDeleted = true;
                await UpdateDriverAsync(driver);
                return true;
            }
            return false;
        }

        public async Task ChangeDriverStatusAsync(int driverId, DriverStatus newStatus)
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
