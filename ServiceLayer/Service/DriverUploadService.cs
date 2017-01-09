using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Enums;
using ServiceLayer.Repository;

namespace ServiceLayer.Service
{
    public class DriverUploadService : EntityService, IDriverUploadService
    {
        private readonly IDriverUploadRepository _driverUploadRepository;
        public DriverUploadService(IEntityRepository entityRepository,
            IDriverUploadRepository repository) : base(entityRepository)
        {
            _driverUploadRepository = repository;
        }

        public async Task<IEnumerable<DriverUpload>> GetDriverUploadsByDriverIdAsync(int id)
        {
            return await _driverUploadRepository.GetDriverUploadByDriverIdAsync(id);
        }

        public async Task<DriverUpload> CreateDriverUploadAsync(DriverUpload driverUpload)
        {
            return await _driverUploadRepository.CreateDriverUpload(driverUpload);
        }

        public async Task ApproveDriverDocumentAsync(int documentId, int personId)
        {
            await _driverUploadRepository.ApproveDriverDocumentAsync(documentId, personId);
        }

        public async Task RejectDriverDocumentAsync(int documentId, int personId, string reason)
        {
            await _driverUploadRepository.RejectDriverDocumentAsync(documentId, personId, reason);
        }

        public async Task<DriverUpload> GetDriverUploadByDriverIdAndUploadTypeAsync(int id, UploadType documentType)
        {
            return await _driverUploadRepository.GetDriverUploadByDriverIdAndUploadTypeAsync(id, documentType);
        }
    }
}
