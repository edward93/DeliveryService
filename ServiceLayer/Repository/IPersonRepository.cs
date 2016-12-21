using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Repository
{
    public interface IPersonRepository : IEntityRepository
    {
        Task<Person> CreatePersonAsync(Person person);
        Task<Person> GetPersonByUserIdAsync(string userId);
    }
}