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
        Task<DriverUpload> GetDriverUploadsByDriverIdAsync(int id);
    }
}
