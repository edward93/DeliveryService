using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using DAL.Context;
using DAL.Entities;
using Infrastructure.Config;
using DeliveryService.Infrastructure;
using ServiceLayer.Repository;
using ServiceLayer.Service;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ServiceLayer.ApplicationService;

namespace DeliveryService.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            container.RegisterType<IDbContext, DbContext>(new PerRequestLifetimeManager());
            container.RegisterType<IConfig, Config>(new PerRequestLifetimeManager());

            // register Repositories
            container.RegisterType<IEntityRepository, EntityRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IAddressRepository, AddressRepository>(new PerRequestLifetimeManager());
            container.RegisterType<ICardRepository, CardRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IRiderRepository, RiderRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IDriverUploadRepository, DriverUploadRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IPersonRepository, PersonRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IBusinessRepository, BusinessRespository>(new PerRequestLifetimeManager());
            container.RegisterType<IOrderRepository, OrderRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IOrderHistoryRepository, OrderHistoryRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IBusinessUploadRepository, BusinessUploadRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IRateRepository, RateRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IDriverLocationRepository, DriverLocationRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IBusinessPenaltyRepository, BusinessPenaltyRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IDriverPenaltyRepository, DriverPenaltyRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IDriverFeeRepository, DriverFeeRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IDiscountRepository, DiscountRepository>(new PerRequestLifetimeManager());

            // register Services
            container.RegisterType<IEntityService, EntityService>(new PerRequestLifetimeManager());
            container.RegisterType<IAddressService, AddressService>(new PerRequestLifetimeManager());
            container.RegisterType<ICardService, CardService>(new PerRequestLifetimeManager());
            container.RegisterType<IRiderService, RiderService>(new PerRequestLifetimeManager());
            container.RegisterType<IDriverUploadService, DriverUploadService>(new PerRequestLifetimeManager());
            container.RegisterType<IPersonService, PersonService>(new PerRequestLifetimeManager());
            container.RegisterType<IBusinessService, BusinessService>(new PerRequestLifetimeManager());
            container.RegisterType<IOrderService, OrderService>(new PerRequestLifetimeManager());
            container.RegisterType<IOrderHistoryService, OrderHistoryService>(new PerRequestLifetimeManager());
            container.RegisterType<IBusinessUploadService, BusinessUploadService>(new PerRequestLifetimeManager());
            container.RegisterType<IRateService, RateService>(new PerRequestLifetimeManager());
            container.RegisterType<IDriverLocationService, DriverLocationService>(new PerRequestLifetimeManager());
            container.RegisterType<IBusinessPenaltyService, BusinessPenaltyService>(new PerRequestLifetimeManager());
            container.RegisterType<IDriverPenaltyService, DriverPenaltyService>(new PerRequestLifetimeManager());
            container.RegisterType<IDriverFeeService, DriverFeeService>(new PerRequestLifetimeManager());
            container.RegisterType<IDiscountService, DiscountService>(new PerRequestLifetimeManager());
            container.RegisterType<IDriverApplicationService, DriverApplicationService>(new PerRequestLifetimeManager());
        }
    }
}
