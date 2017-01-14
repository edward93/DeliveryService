using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using DAL.Context;
using DAL.Entities;

namespace ServiceLayer.Repository
{
    public class BusinessRespository : EntityRepository, IBusinessRepository
    {
        public BusinessRespository(IDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<Business> CreateBusiness(Business business)
        {
            DbContext.Businesses.AddOrUpdate(business);
            await DbContext.SaveChangesAsync();
            return business;
        }

        public Task<IEnumerable<Business>> GetBusinessList()
        {
            throw new System.NotImplementedException();
        }
    }
}
