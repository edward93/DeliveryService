using System;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Enums;
using ServiceLayer.Repository;

namespace ServiceLayer.Service
{
    public class DriverFeeService : EntityService, IDriverFeeService
    {
        private readonly IDriverFeeRepository _driverFeeRepository;
        private readonly Lazy<IRateService> _rateService;

        public DriverFeeService(IEntityRepository entityRepository,
            IDriverFeeRepository driverFeeRepository, 
            IRateService rateService) : base(entityRepository)
        {
            _driverFeeRepository = driverFeeRepository;
            _rateService = new Lazy<IRateService>(() => rateService);
        }

        public async Task CalculateFeeForWaitingAsync(Driver driver, Order order, decimal amount)
        {
            var waitingAsWholeNumber = Math.Truncate(amount);

            var feeAmount =
                await _rateService.Value.GetPaymentByPaymentTypeAsync(PaymentType.DriverPenaltyForDelayPerMinute);

            var fee = new DriverFee
            {
                Amount = feeAmount,
                CreatedBy = driver.Id,
                CreatedDt = DateTime.UtcNow,
                Date = DateTime.UtcNow,
                Description = $"Driver was waiting for business for {waitingAsWholeNumber} minutes.",
                IsDeleted = false,
                FeeType = FeeType.DriverIsWaitingForBusiness,
                UpdatedBy = driver.Id,
                UpdatedDt = DateTime.UtcNow,
                AssociatedOrderId = order.Id,
                DriverId = driver.Id
            };

            await _driverFeeRepository.CreateEntityAsync(fee);
        }
    }
}