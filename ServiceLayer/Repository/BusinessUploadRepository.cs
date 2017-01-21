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
    public class BusinessUploadRepository : EntityRepository, IBusinessUploadRepository
    {
        public BusinessUploadRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<BusinessUpload>> GetBusinessUploadByBusinesIdAsync(int id)
        {
            return await DbContext.BusinessUploads.Where(c => c.BusinessId == id && c.IsDeleted == false).ToListAsync();
        }

        public async Task<BusinessUpload> CreateBusinessUpload(BusinessUpload businessUpload)
        {
            DbContext.BusinessUploads.AddOrUpdate(businessUpload);
            await DbContext.SaveChangesAsync();
            return businessUpload;
        }

        public async Task ApproveBusinessDocumentAsync(int documentId, int personId)
        {
            var doc = await GetByIdAsync<BusinessUpload>(documentId);
            doc.DocumentStatus = DocumentStatus.Approved;
            doc.UpdatedDt = DateTime.UtcNow;
            doc.UpdatedBy = personId;
            DbContext.BusinessUploads.AddOrUpdate(doc);
            await DbContext.SaveChangesAsync();
        }

        public async Task RejectBusinessDocumentAsync(int documentId, int personId, string reason)
        {
            var doc = await GetByIdAsync<BusinessUpload>(documentId);
            doc.DocumentStatus = DocumentStatus.Rejected;
            doc.UpdatedDt = DateTime.UtcNow;
            doc.UpdatedBy = personId;
            doc.RejectionComment = reason;
            DbContext.BusinessUploads.AddOrUpdate(doc);
            await DbContext.SaveChangesAsync();
        }

        public async Task<BusinessUpload> GetBusinessUploadByBusinessIdAndUploadTypeAsync(int id, BusinessUploadType documentType)
        {
            var docs = await GetBusinessUploadByBusinesIdAsync(id);
            return docs.FirstOrDefault(c => c.UploadType == documentType);
        }
    }
}
