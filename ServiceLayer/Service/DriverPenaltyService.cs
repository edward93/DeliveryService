using System;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Enums;
using ServiceLayer.Repository;

namespace ServiceLayer.Service
{
    public class DriverPenaltyService : EntityService, IDriverPenaltyService
    {
        private readonly IDriverPenaltyRepository _driverPenaltyRepository;
        private readonly Lazy<IRateService> _rateService;
        public DriverPenaltyService(IEntityRepository entityRepository, 
            IDriverPenaltyRepository driverPenaltyRepository, 
            IRateService rateService) : base(entityRepository)

        {
            _driverPenaltyRepository = driverPenaltyRepository;
            _rateService = new Lazy<IRateService>(() => rateService);
        }

        public async Task PenalizeDriverForRejectingMoreThenThreeTimesAsync(Driver driver, Order order)
        {
            var fee =
                await _rateService.Value.GetPaymentByPaymentTypeAsync(PaymentType.DriverPenaltyForRejections);

            var penalty = new DriverPenalty
            {
                Amount = fee,
                CreatedBy = driver.Id,
                CreatedDt = DateTime.UtcNow,
                Date = DateTime.UtcNow,
                Description = "Driver rejected or ignored order assigned to him/her more then 3 times during last 24 hours.",
                IsDeleted = false,
                PenaltyType = PenaltyType.DriverThirdRejection,
                UpdatedBy = driver.Id,
                UpdatedDt = DateTime.UtcNow,
                AssociatedOrderId = order.Id,
                DriverId = driver.Id
            };

            await _driverPenaltyRepository.CreateEntityAsync(penalty);
        }

        public async Task CalculatePenaltyForDelayAsync(Driver driver, Order order, decimal driverArrivalDelay)
        {
            var delayAsWholeNumber = Math.Truncate(driverArrivalDelay);

            var fee = 
                await _rateService.Value.GetPaymentByPaymentTypeAsync(PaymentType.DriverPenaltyForDelayPerMinute);

            var penalty = new DriverPenalty
            {
                Amount = fee,
                CreatedBy = driver.Id,
                CreatedDt = DateTime.UtcNow,
                Date = DateTime.UtcNow,
                Description = $"Driver penelized for being late for {delayAsWholeNumber} minutes.",
                IsDeleted = false,
                PenaltyType = PenaltyType.DriverArrivalDelay,
                UpdatedBy = driver.Id,
                UpdatedDt = DateTime.UtcNow,
                AssociatedOrderId = order.Id,
                DriverId = driver.Id
            };

            await _driverPenaltyRepository.CreateEntityAsync(penalty);
        }
    }
}