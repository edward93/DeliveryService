using System.Threading.Tasks;
using DAL.Entities;
using ServiceLayer.Repository;

namespace ServiceLayer.Service
{
    public class AddressService : EntityService, IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        public AddressService(IEntityRepository entityRepository,
            IAddressRepository repository) : base(entityRepository)
        {
            _addressRepository = repository;
        }

        public async Task<Address> GetAddressByDriverIdAsync(int id)
        {
            return await _addressRepository.GetAddressByDriverIdAsync(id);
        }
    }
}