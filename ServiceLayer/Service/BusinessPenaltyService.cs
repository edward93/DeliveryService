using ServiceLayer.Repository;

namespace ServiceLayer.Service
{
    public class BusinessPenaltyService : EntityService, IBusinessPenaltyService
    {
        private readonly IBusinessPenaltyRepository _businessPenaltyRepository;
        public BusinessPenaltyService(IEntityRepository entityRepository, 
            IBusinessPenaltyRepository businessPenaltyRepository) : base(entityRepository)
        {
            _businessPenaltyRepository = businessPenaltyRepository;
        }
    }
}