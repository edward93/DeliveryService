using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Repository
{
    public interface IAddressRepository : IEntityRepository
    {
        Task<Address> GetAddressByDriverIdAsync(int id);
        Task<Address> CreateAddress(Address address);
    }
}