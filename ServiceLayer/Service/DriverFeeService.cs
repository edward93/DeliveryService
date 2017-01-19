using ServiceLayer.Repository;

namespace ServiceLayer.Service
{
    public class DriverFeeService : EntityService, IDriverFeeService
    {
        private readonly IDriverFeeRepository _driverFeeRepository;
        public DriverFeeService(IEntityRepository entityRepository,
            IDriverFeeRepository driverFeeRepository) : base(entityRepository)
        {
            _driverFeeRepository = driverFeeRepository;
        }
    }
}