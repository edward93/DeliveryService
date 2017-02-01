using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Enums;
using ServiceLayer.Repository;

namespace ServiceLayer.Service
{
    public class BusinessUploadService : EntityService, IBusinessUploadService
    {
        private readonly Lazy<IBusinessUploadRepository> _businessUploadRepository;
        public BusinessUploadService(IEntityRepository entityRepository, IBusinessUploadRepository businessUploadRepository) : base(entityRepository)
        {
            _businessUploadRepository = new Lazy<IBusinessUploadRepository>(() => businessUploadRepository);
        }

        public async Task<IEnumerable<BusinessUpload>> GetBusinessUploadsByBusinessIdAsync(int id)
        {
            return await _businessUploadRepository.Value.GetBusinessUploadByBusinesIdAsync(id);
        }

        public async Task<BusinessUpload> CreateBusinessUploadAsync(BusinessUpload businessUpload)
        {
            return await _businessUploadRepository.Value.CreateBusinessUpload(businessUpload);
        }

        public async Task ApproveBusinessDocumentAsync(int documentId, int personId)
        {
            await _businessUploadRepository.Value.ApproveBusinessDocumentAsync(documentId, personId);
        }

        public async Task RejectBusinessDocumentAsync(int documentId, int personId, string reason)
        {
            await _businessUploadRepository.Value.RejectBusinessDocumentAsync(documentId, personId, reason);
        }

        public async Task<BusinessUpload> GetBusinessUploadByBusinessIdAndUploadTypeAsync(int id, BusinessUploadType documentType)
        {
            return await _businessUploadRepository.Value.GetBusinessUploadByBusinessIdAndUploadTypeAsync(id, documentType);
        }
    }
}
