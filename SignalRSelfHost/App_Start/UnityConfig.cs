using DAL.Context;
using Microsoft.AspNet.SignalR;
using Microsoft.Practices.Unity;
using ServiceLayer.Repository;
using ServiceLayer.Service;
using SignalRSelfHost.Resolver;

namespace SignalRSelfHost.App_Start
{
    public class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            container.RegisterType<IEntityRepository, EntityRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IDriverRepository, DriverRepository>(new HierarchicalLifetimeManager());

            container.RegisterType<IEntityService, EntityService>(new HierarchicalLifetimeManager());
            container.RegisterType<IDriverService, DriverService>(new HierarchicalLifetimeManager());

        }
        public static void Initialise() // this isn't my misspelling, it's in the Unity.MVC NuGet package.  
        {
            var container = BuildUnityContainer();
            GlobalHost.DependencyResolver = new SignalRUnityDependencyResolver(container);
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
            container.RegisterType<AddRiderHub.AddRiderHub>(new InjectionFactory(CreateMyHub));

            return container;
        }

        private static object CreateMyHub(IUnityContainer p)
        {
            var myHub = new AddRiderHub.AddRiderHub(p.Resolve<IOrderService>());

            return myHub;
        }
    }
}