using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Service
{
    public interface IDriverPenaltyService : IEntityService
    {
        Task PenalizeDriverForRejectingMoreThenThreeTimesAsync(Driver driver, Order order);
        Task CalculatePenaltyForDelayAsync(Driver driver, Order order, decimal driverArrivalDelay);
    }
}