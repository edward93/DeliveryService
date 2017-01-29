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
        private static readonly Config Config;

        static AddRiderSignalR()
        {
            Config = new Config();
        }
        static void Main(string[] args)
        {
            // This will *ONLY* bind to localhost, if you want to bind to all addresses
            // use http://*:8080 to bind to all addresses. 
            // See http://msdn.microsoft.com/en-us/library/system.net.httplistener.aspx 
            // for more information.

            StartServer();
        }
        public static void StartServer()
        {
            string url = $"{Config.ServerUrl}:{Config.ServerPort}";
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("Server running on {0}", url);
                Console.ReadLine();
            }
        }
    }
}
