using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Enums;

namespace ServiceLayer.Service
{
    public interface IBusinessUploadService : IEntityService
    {
        Task<IEnumerable<BusinessUpload>> GetBusinessUploadsByBusinessIdAsync(int id);
        Task<BusinessUpload> CreateBusinessUploadAsync(BusinessUpload businessUpload);
        Task ApproveBusinessDocumentAsync(int documentId, int personId);
        Task RejectBusinessDocumentAsync(int documentId, int personId, string reason);
        Task<BusinessUpload> GetBusinessUploadByBusinessIdAndUploadTypeAsync(int id, BusinessUploadType documentType);
    }
}
