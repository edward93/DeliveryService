using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;
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
    }
}
