using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Repository
{
    public interface ICardRepository : IEntityRepository
    {
        Task<Card> GetCardByDriverIdAsync(int id);
    }
}