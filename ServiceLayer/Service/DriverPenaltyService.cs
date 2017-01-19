using ServiceLayer.Repository;

namespace ServiceLayer.Service
{
    public class DriverPenaltyService : EntityService, IDriverPenaltyService
    {
        private readonly IDriverPenaltyRepository _driverPenaltyRepository;
        public DriverPenaltyService(IEntityRepository entityRepository, 
            IDriverPenaltyRepository driverPenaltyRepository) : base(entityRepository)

        {
            _driverPenaltyRepository = driverPenaltyRepository;
        }
    }
}