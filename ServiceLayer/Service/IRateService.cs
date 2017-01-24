using System.Threading.Tasks;
using DAL.Enums;

namespace ServiceLayer.Service
{
    public interface IRateService : IEntityService
    {
        Task<decimal> GetPaymentByPaymentTypeAsync(PaymentType paymentType);
    }
}