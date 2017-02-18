using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Web.Configuration;
using Infrastructure.Config;

namespace DeliveryService.Infrastructure
{
    public class Config : IConfig
    {
        public NameValueCollection Messages => (NameValueCollection)ConfigurationManager.GetSection("Messages");
        public string UploadsFolderPath => WebConfigurationManager.AppSettings["UploadFolderPath"];
        public string WebApiUrl => WebConfigurationManager.AppSettings["WebApiUrl"];
        public string DistanceMatrixApiUrl => WebConfigurationManager.AppSettings["GoogleMapsDistanseMatrixApiUrl"];
        public string SignalRServerUrl => WebConfigurationManager.AppSettings["SignalRServerUrl"];
        public string SignalRServerPort => WebConfigurationManager.AppSettings["SignalRServerPort"];
        public string GoogleMapsApiKey => WebConfigurationManager.AppSettings["GoogleMapsApiKey"];
    }
}