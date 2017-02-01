using System;
using System.Data.Entity;
using System.Threading.Tasks;
using DAL.Context;
using DAL.Entities;
using DAL.Enums;

namespace ServiceLayer.Repository
{
    public class DiscountRepository : EntityRepository, IDiscountRepository
    {
        public DiscountRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Discount> GetDiscountByBusinessIdForGivenPaymentTypeAsync(int businessId, PaymentType paymentType)
        {
            var discount = await DbContext.Discounts.FirstOrDefaultAsync(c => c.BusinessId == businessId && c.Rate.PaymentType == paymentType);

            if (discount == null) throw new Exception($"There is no discount found for the business with id: {businessId}");

            return discount;
        }
    }
}