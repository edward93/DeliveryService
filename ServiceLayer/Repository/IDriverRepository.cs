using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;
using DeliveryService.Models.ViewModels;

namespace ServiceLayer.Repository
{
    public interface IDriverRepository: IEntityRepository
    {
        Task<Driver> CreateDriverAsync(Driver driver);
        Task<Driver> GetDriverByPersonId(string personId);
        Task ApproveDriver(int driverId);
        Task<int> GetOnlineDriversCountAsync();
        Task<IEnumerable<DriverDetailsWithLocation>> GetOnlineDriversAsync();
        Task<IEnumerable<DriverDetailsWithLocation>> GetBusinessDriversAsync(int businessId);
    }
}
