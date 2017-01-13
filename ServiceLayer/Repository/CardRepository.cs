using System.Data.Entity;
using System.Threading.Tasks;
using DAL.Context;
using DAL.Entities;

namespace ServiceLayer.Repository
{
    public class CardRepository : EntityRepository, ICardRepository
    {
        public CardRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Card> GetCardByDriverIdAsync(int id)
        {
            return await DbContext.Cards.FirstOrDefaultAsync(c => c.EntityId == id);
        }
    }
}