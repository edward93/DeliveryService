using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Repository
{
    public interface IBusinessRepository : IEntityRepository
    {
        Task<Business> CreateBusiness(Business business);
        Task<IEnumerable<Business>> GetBusinessList();
        
    }
}
