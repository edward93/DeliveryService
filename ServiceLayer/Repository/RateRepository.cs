using DAL.Context;

namespace ServiceLayer.Repository
{
    public class RateRepository : EntityRepository, IRateRepository
    {
        public RateRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}