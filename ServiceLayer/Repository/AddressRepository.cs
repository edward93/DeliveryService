using System.Data.Entity;
using System.Threading.Tasks;
using DAL.Context;
using DAL.Entities;

namespace ServiceLayer.Repository
{
    public class AddressRepository : EntityRepository, IAddressRepository
    {
        public AddressRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Address> GetAddressByDriverIdAsync(int id)
        {
            return await DbContext.Addresses.FirstOrDefaultAsync(c => c.DriverId == id);
        }
    }
}