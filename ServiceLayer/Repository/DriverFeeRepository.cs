using DAL.Context;

namespace ServiceLayer.Repository
{
    public class DriverFeeRepository : EntityRepository, IDriverFeeRepository
    {
        public DriverFeeRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}