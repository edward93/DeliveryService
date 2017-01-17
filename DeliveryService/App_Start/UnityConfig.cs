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

            // TODO: Register your types here
            container.RegisterType<IDbContext, DbContext>(new PerRequestLifetimeManager());
            container.RegisterType<IConfig, Config>(new PerRequestLifetimeManager());

            // register Repositories
            container.RegisterType<IEntityRepository, EntityRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IAddressRepository, AddressRepository>(new PerRequestLifetimeManager());
            container.RegisterType<ICardRepository, CardRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IDriverRepository, DriverRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IDriverUploadRepository, DriverUploadRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IPersonRepository, PersonRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IBusinessRepository, BusinessRespository>(new PerRequestLifetimeManager());

            // register Services
            container.RegisterType<IEntityService, EntityService>(new PerRequestLifetimeManager());
            container.RegisterType<IAddressService, AddressService>(new PerRequestLifetimeManager());
            container.RegisterType<ICardService, CardService>(new PerRequestLifetimeManager());
            container.RegisterType<IDriverService, DriverService>(new PerRequestLifetimeManager());
            container.RegisterType<IDriverUploadService, DriverUploadService>(new PerRequestLifetimeManager());
            container.RegisterType<IPersonService, PersonService>(new PerRequestLifetimeManager());
            container.RegisterType<IBusinessService, BusinessService>(new PerRequestLifetimeManager());
        }
    }
}
