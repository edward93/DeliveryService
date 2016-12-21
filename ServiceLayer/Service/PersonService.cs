using System.Threading.Tasks;
using DAL.Entities;
using ServiceLayer.Repository;

namespace ServiceLayer.Service
{
    public class PersonService : EntityService, IPersonService
    {
        private readonly IPersonRepository _personRepository;
        public PersonService(IEntityRepository entityRepository,
            IPersonRepository personRepository) : base(entityRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<Person> CreatePersonAsync(Person person)
        {
            return await _personRepository.CreatePersonAsync(person);
        }

        public async Task<Person> GetPersonByUserIdAsync(string userId)
        {
            return await _personRepository.GetPersonByUserIdAsync(userId);
        }
    }
}