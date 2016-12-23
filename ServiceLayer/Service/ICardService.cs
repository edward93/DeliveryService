using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Service
{
    public interface ICardService : IEntityService
    {
        Task<Card> GetCardByDriverIdAsync(int id);
    }
}