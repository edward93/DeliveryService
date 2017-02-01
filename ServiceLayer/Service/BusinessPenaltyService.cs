using System;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Enums;
using ServiceLayer.Repository;

namespace ServiceLayer.Service
{
    public class BusinessPenaltyService : EntityService, IBusinessPenaltyService
    {
        private readonly IBusinessPenaltyRepository _businessPenaltyRepository;
        private readonly Lazy<IRateService> _rateService;

        public BusinessPenaltyService(IEntityRepository entityRepository, 
            IBusinessPenaltyRepository businessPenaltyRepository,
            IRateService rateService) : base(entityRepository)
        {
            _businessPenaltyRepository = businessPenaltyRepository;
            _rateService = new Lazy<IRateService>(() => rateService);
        }

        public async Task CalculatePenaltyForDriverWaitingAsync(Driver driver, Order order, decimal amount)
        {
            var waitingAsWholeNumber = Math.Truncate(amount);

            var feeAmount =
                await _rateService.Value.GetPaymentByPaymentTypeAsync(PaymentType.DriverPenaltyForDelayPerMinute);

            var penalty = new BusinessPenalty
            {
                Amount = feeAmount,
                CreatedBy = driver.Id,
                CreatedDt = DateTime.UtcNow,
                Date = DateTime.UtcNow,
                Description = $"Driver was waiting for business for {waitingAsWholeNumber} minutes.",
                IsDeleted = false,
                PenaltyType = PenaltyType.DriverIsWaitingForBusiness,
                UpdatedBy = order.Business.ContactPerson.Id,
                UpdatedDt = DateTime.UtcNow,
                AssociatedOrderId = order.Id,
                BusinessId = order.Business.Id
            };

            await _businessPenaltyRepository.CreateEntityAsync(penalty);
        }
    }
}