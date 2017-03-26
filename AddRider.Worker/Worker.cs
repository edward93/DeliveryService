using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddRider.Worker.Resolver;
using AddRider.Worker.Workers;
using DAL.Context;
using Microsoft.AspNet.SignalR;
using Microsoft.Practices.Unity;
using ServiceLayer.Repository;
using ServiceLayer.Service;

namespace AddRider.Worker
{
    class Worker
    {
        static IUnityContainer _container;
        static void Main(string[] args)
        {
            // init unity
            InitializeUnity();

            Console.WriteLine($"{nameof(Worker)} has started.");
            // Run worker processes
            StartWorkerProcesses(args).Wait();

            // This line should be removed
            Console.ReadLine();
        }

        private static void LoadContainer()
        {
            _container = new UnityContainer();

            // Register types
            _container.RegisterType<IDbContext, DbContext>();
            // Register services
            _container.RegisterType<IRiderService, RiderService>();
            _container.RegisterType<IOrderApplicationService, OrderApplicationService>();
            _container.RegisterType<IOrderService, OrderService>();
            _container.RegisterType<IOrderHistoryService, OrderHistoryService>();
            _container.RegisterType<IEntityService, EntityService>();
            _container.RegisterType<IRateService, RateService>();
            _container.RegisterType<IRiderPenaltyService, RiderPenaltyService>();


            // Register repositories
            _container.RegisterType<IEntityRepository, EntityRepository>();
            _container.RegisterType<IRiderRepository, RiderRepository>();
            _container.RegisterType<IOrderRepository, OrderRepository>();
            _container.RegisterType<IOrderHistoryRepository, OrderHistoryRepository>();
            _container.RegisterType<IRateRepository, RateRepository>();
            _container.RegisterType<IDriverPenaltyRepository, DriverPenaltyRepository>();
        }

        public static void InitializeUnity()
        {
            LoadContainer();
            GlobalHost.DependencyResolver = new WorkerDependencyResolver(_container);
        }

        private static async Task StartWorkerProcesses(string[] args)
        {
            try
            {
                var worker = new RiderWorkerProcess(
                    _container.Resolve<IRiderService>(),
                    _container.Resolve<IOrderApplicationService>()
                    );
                await worker.RiderStatusProcessing();
                await worker.RejectOrderAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
    }
}
