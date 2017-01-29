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
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using DeliveryService.API.Resolver;
using DeliveryService.API.Hubs;

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
            container.RegisterType<IBusinessRepository, BusinessRespository>(new HierarchicalLifetimeManager());
            container.RegisterType<IOrderRepository, OrderRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IOrderHistoryRepository, OrderHistoryRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IBusinessUploadRepository, BusinessUploadRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IRateRepository, RateRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IDriverLocationRepository, DriverLocationRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IBusinessPenaltyRepository, BusinessPenaltyRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IDriverPenaltyRepository, DriverPenaltyRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IDriverFeeRepository, DriverFeeRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IDiscountRepository, DiscountRepository>(new HierarchicalLifetimeManager());

            // register Services
            container.RegisterType<IEntityService, EntityService>(new HierarchicalLifetimeManager());
            container.RegisterType<IAddressService, AddressService>(new HierarchicalLifetimeManager());
            container.RegisterType<ICardService, CardService>(new HierarchicalLifetimeManager());
            container.RegisterType<IDriverService, DriverService>(new HierarchicalLifetimeManager());
            container.RegisterType<IDriverUploadService, DriverUploadService>(new HierarchicalLifetimeManager());
            container.RegisterType<IPersonService, PersonService>(new HierarchicalLifetimeManager());
            container.RegisterType<IBusinessService, BusinessService>(new HierarchicalLifetimeManager());
            container.RegisterType<IOrderService, OrderService>(new HierarchicalLifetimeManager());
            container.RegisterType<IOrderHistoryService, OrderHistoryService>(new HierarchicalLifetimeManager());
            container.RegisterType<IBusinessUploadService, BusinessUploadService>(new HierarchicalLifetimeManager());
            container.RegisterType<IRateService, RateService>(new HierarchicalLifetimeManager());
            container.RegisterType<IDriverLocationService, DriverLocationService>(new HierarchicalLifetimeManager());
            container.RegisterType<IBusinessPenaltyService, BusinessPenaltyService>(new HierarchicalLifetimeManager());
            container.RegisterType<IDriverPenaltyService, DriverPenaltyService>(new HierarchicalLifetimeManager());
            container.RegisterType<IDriverFeeService, DriverFeeService>(new HierarchicalLifetimeManager());
            container.RegisterType<IDiscountService, DiscountService>(new HierarchicalLifetimeManager());


        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();
            container.RegisterType<IDbContext, DbContext>();
            container.RegisterType<IEntityRepository, EntityRepository>();
            container.RegisterType<IEntityService, EntityService>();
            // register all your dependencies here.
            container.RegisterType<IOrderHistoryRepository, OrderHistoryRepository>();
            container.RegisterType<IRateService, RateService>();
            container.RegisterType<IRateRepository, RateRepository>();
            container.RegisterType<IOrderHistoryService, OrderHistoryService>();
            container.RegisterType<IOrderRepository, OrderRepository>();
            container.RegisterType<IOrderService, OrderService>();
            container.RegisterType<AddRiderHub>(new InjectionFactory(CreateMyHub));

            return container;
        }

        private static object CreateMyHub(IUnityContainer p)
        {
            var myHub = new AddRiderHub(p.Resolve<IOrderService>());

            return myHub;
        }

        public static void Initialise() // this isn't my misspelling, it's in the Unity.MVC NuGet package.  
        {
            var container = BuildUnityContainer();

           // var unityDependencyResolver = new UnityDependencyResolver(container);

            // used for MVC
         //   DependencyResolver.SetResolver(unityDependencyResolver);
            // used for WebAPI
          //  GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
            // used for SignalR
            GlobalHost.DependencyResolver = new SignalRUnityDependencyResolver(container);
        }
    }
}