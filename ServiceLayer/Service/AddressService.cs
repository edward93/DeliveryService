using System;
using System.Threading.Tasks;
using DAL.Entities;
using ServiceLayer.Repository;

namespace ServiceLayer.Service
{
    public class AddressService : EntityService, IAddressService
    {
        private readonly Lazy<IAddressRepository> _addressRepository;
        public AddressService(IEntityRepository entityRepository,
            IAddressRepository repository) : base(entityRepository)
        {
            _addressRepository = new Lazy<IAddressRepository>(() => repository);
        }

        public async Task<Address> GetAddressByDriverIdAsync(int id)
        {
            return await _addressRepository.Value.GetAddressByDriverIdAsync(id);
        }
        public async Task<Address> CreateAddress(Address address)
        {
            return await _addressRepository.Value.CreateAddress(address);
        }
    }
}