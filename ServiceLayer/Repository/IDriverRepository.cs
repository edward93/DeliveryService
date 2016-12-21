using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Repository
{
    public interface IDriverRepository: IEntityRepository
    {
        Task<IEnumerable<Driver>> GetDriversList();
        Task<Driver> GetDriverById(int id);
        Task<Driver> AddDriver(Driver driver);
    }
}
