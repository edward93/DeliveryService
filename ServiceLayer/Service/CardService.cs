using System;
using System.Threading.Tasks;
using DAL.Entities;
using ServiceLayer.Repository;

namespace ServiceLayer.Service
{
    public class CardService : EntityService, ICardService
    {
        private readonly ICardRepository _cardRepository;
        public CardService(IEntityRepository entityRepository,
            ICardRepository repository,
            IAddressRepository addressRepository) : base(entityRepository)
        {
            _cardRepository = repository;
        }

        public async Task<Card> GetCardByDriverIdAsync(int id)
        {
            return await _cardRepository.GetCardByDriverIdAsync(id);
        }
    }
}