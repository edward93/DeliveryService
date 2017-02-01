using System.Collections.Specialized;
using System.Configuration;
using Infrastructure.Config;

namespace SignalRSelfHost.Infrastructure
{
    public class Config : IConfig
    {
        public NameValueCollection Messages { get; }
        public string UploadsFolderPath => ConfigurationManager.AppSettings["UploadFolderPath"];
        public string WebApiUrl { get; }
        public string DistanceMatrixApiUrl { get; }

        public string SignalRServerUrl => ConfigurationManager.AppSettings["SignalRServerUrl"];
        public string SignalRServerPort => ConfigurationManager.AppSettings["SignalRServerPort"];
    }
}