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

        public async Task<Driver> GetDriverById(int id)
        {
            return await _driverRepository.GetDriverById(id);
        }

        public async Task<IEnumerable<Driver>> GetDrivers()
        {
            return await _driverRepository.GetDriversList();
        }

        public async Task<Driver> AddDriver(Driver driver)
        {
            return await _driverRepository.AddDriver(driver);
        }
    }
}
