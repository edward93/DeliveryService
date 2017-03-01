using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.ApplicationService
{
    public interface IDriverApplicationService
    {
        Task<Driver> GetNearestDriverAsync(Order order);
    }
}