using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Enums;

namespace ServiceLayer.Repository
{
    public interface IDriverUploadRepository : IEntityRepository
    {
        Task<IEnumerable<DriverUpload>> GetDriverUploadByDriverIdAsync(int id);
        Task<DriverUpload> CreateDriverUpload(DriverUpload driverUpload);
        Task ApproveDriverDocumentAsync(int documentId, int personId);
        Task RejectDriverDocumentAsync(int documentId, int personId, string reason);
        Task<DriverUpload> GetDriverUploadByDriverIdAndUploadTypeAsync(int id, UploadType documentType);
    }
}
