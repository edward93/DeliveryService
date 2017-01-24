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
    }
}