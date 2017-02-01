using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Service
{
    public interface IDriverFeeService : IEntityService
    {
        Task CalculateFeeForWaitingAsync(Driver driver, Order order, decimal amount);
    }
}