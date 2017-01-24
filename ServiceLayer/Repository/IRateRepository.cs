using System.Threading.Tasks;
using DAL.Enums;

namespace ServiceLayer.Repository
{
    public interface IRateRepository : IEntityRepository
    {
        Task<decimal> GetPaymentByPaymentTypeAsync(PaymentType paymentType);
    }
}