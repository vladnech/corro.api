using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;

namespace Correspondence.Api.Config
{
    public class FunctionSettings : IFunctionSettings
    {
        public FunctionSettings(ExecutionContextOptions context)
        {
            var config = new ConfigurationBuilder()
                                .SetBasePath(context.AppDirectory)
                                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true) // <- Your application settings in your local development environment
                                .AddEnvironmentVariables() // <- Gets you the application settings in Azure
                                .Build();
            CosmosDBEndpoint = config["COSMOSDB_ENDPOINT"];
            CosmosDBMasterKey = config["COSMOSDB_MASTER_KEY"];
            CosmosDBName = config["COSMOSDB_NAME"];
            int cacheInMinutes;
            if (int.TryParse(config["CacheInMinutes"], out cacheInMinutes))
                CacheInMinutes = cacheInMinutes;
            else
                CacheInMinutes = 10;
        }
        public string CosmosDBEndpoint { get; private set; }
        public string CosmosDBMasterKey { get; private set; }
        public string CosmosDBName { get; private set; }
        public int CacheInMinutes { get; private set; }
    }
}
