using System.Threading.Tasks;
using DAL.Entities;
using DAL.Enums;

namespace ServiceLayer.Repository
{
    public interface IDiscountRepository : IEntityRepository
    {
        Task<Discount> GetDiscountByBusinessIdForGivenPaymentTypeAsync(int businessId, PaymentType paymentType);
    }
}