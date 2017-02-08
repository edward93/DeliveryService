using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;
using Infrastructure.Config;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using SignalRSelfHost.App_Start;
using SignalRSelfHost.Infrastructure;

namespace SignalRSelfHost
{
    class AddRiderSignalR
    {
        private static readonly IConfig Config;

        static AddRiderSignalR()
        {
            Config = new Config();
        }
        static void Main(string[] args)
        {
            try
            {
                StartServer();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void StartServer()
        {
            string url = $"{Config.SignalRServerUrl}:{Config.SignalRServerPort}";
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("Server running on {0}", url);
                Console.ReadLine();
            }
        }
    }
}
