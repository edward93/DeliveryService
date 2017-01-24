using DAL.Context;

namespace ServiceLayer.Repository
{
    public class DiscountRepository : EntityRepository, IDiscountRepository
    {
        public DiscountRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}