using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Web.Configuration;
using Infrastructure.Config;

namespace DeliveryService.Infrastructure
{
    public class Config : IConfig
    {
        public int TestNumber => Convert.ToInt32(WebConfigurationManager.AppSettings["TestNumber"]);
        public NameValueCollection Messages => (NameValueCollection)ConfigurationManager.GetSection("Messages");
    }
}