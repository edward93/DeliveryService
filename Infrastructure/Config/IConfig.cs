using System.Collections.Specialized;

namespace Infrastructure.Config
{
    public interface IConfig
    {
        // For demonstrating how this works. See the web porject under Infrasturcture folder
        int TestNumber { get; }
        NameValueCollection Messages { get; }
    }
}