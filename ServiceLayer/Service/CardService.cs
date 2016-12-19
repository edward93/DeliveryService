using System;
using System.Threading.Tasks;
using DAL.Entities;
using ServiceLayer.Repository;

namespace ServiceLayer.Service
{
    public class CardService : EntityService, ICardService
    {
        private readonly ICardRepository _cardRepository;
        //private readonly Lazy<IAddressRepository> _addressRepository;
        public CardService(IEntityRepository entityRepository,
            ICardRepository repository,
            IAddressRepository addressRepository) : base(entityRepository)
        {
            _cardRepository = repository;
            //_addressRepository = new Lazy<IAddressRepository>(() => addressRepository);
        }

        public async Task<Card> GetCardByDriverIdAsync(int id)
        {
            //await _addressRepository.Value.GetAddressByDriverIdAsync(123);
            return await _cardRepository.GetCardByDriverIdAsync(id);
        }
    }
}