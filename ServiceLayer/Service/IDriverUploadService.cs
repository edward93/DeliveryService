using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Service
{
    public interface IDriverUploadService : IEntityService
    {
        Task<IEnumerable<DriverUpload>> GetDriverUploadsByDriverIdAsync(int id);
        Task<DriverUpload> CreateDriverUpload(DriverUpload driverUpload);
    }
}
