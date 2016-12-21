using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Service
{
    public interface IPersonService : IEntityService
    {
        Task<Person> CreatePersonAsync(Person person);
        Task<Person> GetPersonByUserIdAsync(string getUserId);
    }
}