using Microsoft.Practices.Unity;
using System.Web.Http;
using Infrastructure.Config;
using ServiceLayer.Repository;
using ServiceLayer.Service;
using Unity.WebApi;
using DeliveryService.API.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using DAL.Entities;
using DAL.Context;

namespace DeliveryService.API
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            // e.g. container.RegisterType<ITestService, TestService>();
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);

            container.RegisterType<IDbContext, DbContext>(new HierarchicalLifetimeManager());
            container.RegisterType<IConfig, Config>(new HierarchicalLifetimeManager());

            // register Repositories
            container.RegisterType<IEntityRepository, EntityRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IAddressRepository, AddressRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<ICardRepository, CardRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IDriverRepository, DriverRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IDriverUploadRepository, DriverUploadRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IPersonRepository, PersonRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IDriverLocationRepository, DriverLocationRepository>(new HierarchicalLifetimeManager());

            // register Services
            container.RegisterType<IEntityService, EntityService>(new HierarchicalLifetimeManager());
            container.RegisterType<IAddressService, AddressService>(new HierarchicalLifetimeManager());
            container.RegisterType<ICardService, CardService>(new HierarchicalLifetimeManager());
            container.RegisterType<IDriverService, DriverService>(new HierarchicalLifetimeManager());
            container.RegisterType<IDriverUploadService, DriverUploadService>(new HierarchicalLifetimeManager());
            container.RegisterType<IPersonService, PersonService>(new HierarchicalLifetimeManager());
            container.RegisterType<IDriverLocationService, DriverLocationService>(new HierarchicalLifetimeManager());
        }
    }
}