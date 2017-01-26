using System.Threading.Tasks;
using DAL.Entities;
using DAL.Enums;

namespace ServiceLayer.Service
{
    public interface IDiscountService : IEntityService
    {
        Task<Discount> GetDiscountByBusinessIdForGivenPaymentTypeAsync(int businessId, PaymentType paymentType);
    }
}