using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Context;
using DAL.Entities;

namespace ServiceLayer.Repository
{
    public class DriverUploadRepository : EntityRepository, IDriverUploadRepository
    {
        public DriverUploadRepository(IDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<IEnumerable<DriverUpload>> GetDriverUploadByDriverIdAsync(int id)
        {
            return await DbContext.DriverUploads.Where(c => c.DriverId == id).ToListAsync();
        }

        public async Task<DriverUpload> CreateDriverUpload(DriverUpload driverUpload)
        {
            DbContext.DriverUploads.AddOrUpdate(driverUpload);
            await DbContext.SaveChangesAsync();
            return driverUpload;
        }
    }
}
