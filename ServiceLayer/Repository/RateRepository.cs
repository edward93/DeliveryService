using System;
using System.Data.Entity;
using System.Threading.Tasks;
using DAL.Context;
using DAL.Enums;

namespace ServiceLayer.Repository
{
    public class RateRepository : EntityRepository, IRateRepository
    {
        public RateRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<decimal> GetPaymentByPaymentTypeAsync(PaymentType paymentType)
        {
            var rate =  await DbContext.Rates.FirstOrDefaultAsync(c => c.PaymentType == paymentType);

            if (rate == null) throw new Exception($"No rate found for given payment type: {paymentType}.");

            return rate.Amount;
        }
    }
}