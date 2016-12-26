using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities;
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

        public async Task<DriverUpload> CreateDriverUpload(DriverUpload driverUpload)
        {
            return await _driverUploadRepository.CreateDriverUpload(driverUpload);
        }
    }
}
