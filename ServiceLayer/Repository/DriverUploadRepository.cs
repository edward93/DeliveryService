using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Context;
using DAL.Entities;
using DAL.Enums;

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

        public async Task ApproveDriverDocumentAsync(int documentId, int personId)
        {
            var doc = await GetByIdAsync<DriverUpload>(documentId);
            doc.DocumentStatus = DocumentStatus.Approved;
            doc.UpdatedDt = DateTime.UtcNow;
            doc.UpdatedBy = personId;
            DbContext.DriverUploads.AddOrUpdate(doc);
            await DbContext.SaveChangesAsync();
        }

        public async Task RejectDriverDocumentAsync(int documentId, int personId, string reason)
        {
            var doc = await GetByIdAsync<DriverUpload>(documentId);
            doc.DocumentStatus = DocumentStatus.Rejected;
            doc.UpdatedDt = DateTime.UtcNow;
            doc.UpdatedBy = personId;
            doc.RejectionComment = reason;
            DbContext.DriverUploads.AddOrUpdate(doc);
            await DbContext.SaveChangesAsync();
        }
    }
}
