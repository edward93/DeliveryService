using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Service
{
    public interface IBusinessService : IEntityService
    {
        Task<Business> CreateBusiness(Business address);
    }
}
