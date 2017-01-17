using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Service
{
    public interface IAddressService : IEntityService
    {
        Task<Address> GetAddressByDriverIdAsync(int id);
        Task<Address> CreateAddress(Address address);
    }
}