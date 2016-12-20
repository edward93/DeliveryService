using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public async Task<DriverUpload> GetDriverUploadByDriverIdAsync(int id)
        {
            return await DbContext.DriverUploads.FirstOrDefaultAsync(c => c.DriverId == id);
        }
    }
}
