using System.Collections.Specialized;

namespace Infrastructure.Config
{
    public interface IConfig
    {
        // For demonstrating how this works. See the web porject under Infrasturcture folder
        NameValueCollection Messages { get; }
        string UploadsFolderPath { get; }
        string WebApiUrl { get; }
    }
}