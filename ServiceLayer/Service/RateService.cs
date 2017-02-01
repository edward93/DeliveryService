using System.Threading.Tasks;
using DAL.Enums;
using ServiceLayer.Repository;

namespace ServiceLayer.Service
{
    public class RateService : EntityService, IRateService
    {
        private readonly IRateRepository _rateRepository;
        public RateService(IEntityRepository entityRepository,
            IRateRepository repository) : base(entityRepository)
        {
            _rateRepository = repository;
        }

        public async Task<decimal> GetPaymentByPaymentTypeAsync(PaymentType paymentType)
        {
            return await _rateRepository.GetPaymentByPaymentTypeAsync(paymentType);
        }
    }
}