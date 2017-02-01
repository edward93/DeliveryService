using System.Collections.Specialized;

namespace Infrastructure.Config
{
    public interface IConfig
    {
        NameValueCollection Messages { get; }
        string UploadsFolderPath { get; }
        string WebApiUrl { get; }
        string DistanceMatrixApiUrl { get; }
        string SignalRServerUrl { get; }
        string SignalRServerPort { get; }
    }
}