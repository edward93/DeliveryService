using DAL.Context;

namespace ServiceLayer.Repository
{
    public class DriverPenaltyRepository : EntityRepository, IDriverPenaltyRepository
    {
        public DriverPenaltyRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}