using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Service
{
    public interface IDriverService
    {
        Task<Driver> GetDriverById(int id);
        Task<IEnumerable<Driver>> GetDrivers();
    }
}
