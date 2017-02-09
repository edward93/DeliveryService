using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Service
{
    public interface IBusinessPenaltyService : IEntityService
    {
        Task CalculatePenaltyForDriverWaitingAsync(Driver driver, Order order, decimal driverWaitingTime);
        Task PenelizeBusinessForRejectionAsync(Driver driver, Order order);
    }
}