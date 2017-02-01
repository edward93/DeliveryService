using DAL.Context;

namespace ServiceLayer.Repository
{
    public class BusinessPenaltyRepository : EntityRepository, IBusinessPenaltyRepository
    {
        public BusinessPenaltyRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}