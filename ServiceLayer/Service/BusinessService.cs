using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;
using ServiceLayer.Repository;

namespace ServiceLayer.Service
{
    public class BusinessService : EntityService, IBusinessService
    {
        private readonly Lazy<IBusinessRepository> _businessRepository;

        public BusinessService(IEntityRepository entityRepository, IBusinessRepository businessRepository)
            : base(entityRepository)
        {
            _businessRepository = new Lazy<IBusinessRepository>(() => businessRepository);
        }

        public async Task<Business> CreateBusiness(Business business)
        {
            return await _businessRepository.Value.CreateBusiness(business);
        }

        public async Task<Business> GetBusinessByPersonId(int personId)
        {
            return await _businessRepository.Value.GetBusinessByPersonIdAsync(personId);
        }
    }
}
