using System.Threading.Tasks;
using DAL.Entities;
using DAL.Enums;
using ServiceLayer.Repository;

namespace ServiceLayer.Service
{
    public class DiscountService : EntityService, IDiscountService
    {
        private readonly IDiscountRepository _discountRepository;
        public DiscountService(IEntityRepository entityRepository, 
            IDiscountRepository discountRepository) : base(entityRepository)
        {
            _discountRepository = discountRepository;
        }

        public async Task<Discount> GetDiscountByBusinessIdForGivenPaymentTypeAsync(int businessId, PaymentType paymentType)
        {
            return await _discountRepository.GetDiscountByBusinessIdForGivenPaymentTypeAsync(businessId, paymentType);
        }
    }
}