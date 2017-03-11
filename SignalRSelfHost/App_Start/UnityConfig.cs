using DAL.Context;
using Microsoft.AspNet.SignalR;
using Microsoft.Practices.Unity;
using ServiceLayer.ApplicationService;
using ServiceLayer.Repository;
using ServiceLayer.Service;
using SignalRSelfHost.Resolver;

namespace SignalRSelfHost.App_Start
{
    public class UnityConfig
    {
       /* public static void RegisterComponents()
        {
            var container = new UnityContainer();

            container.RegisterType<IEntityRepository, EntityRepository>();
            container.RegisterType<IDriverRepository, DriverRepository>();

            container.RegisterType<IEntityService, EntityService>();
            container.RegisterType<IDriverService, DriverService>();

        }*/
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
            container.RegisterType<IDriverApplicationService, DriverApplicationService>();
            container.RegisterType<AddRiderHub.AddRiderHub>(new InjectionFactory(CreateMyHub));

            return container;
        }

        private static object CreateMyHub(IUnityContainer p)
        {
            var myHub = new AddRiderHub.AddRiderHub(p.Resolve<IOrderService>(), p.Resolve<IRiderService>());

            return myHub;
        }
    }
}