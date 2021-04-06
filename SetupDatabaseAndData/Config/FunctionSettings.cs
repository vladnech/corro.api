using System.IO;
using System.Text.Json;

namespace SetupDatabaseAndData.Config
{
    public class FunctionSettings : IFunctionSettings
    {
        public FunctionSettings()
        {
            var settingsJson = File.ReadAllText("local.settings.json");
            var settings = JsonDocument.Parse(settingsJson);
            var settingValues = settings.RootElement.GetProperty("Values");
            CosmosDBEndpoint = settingValues.GetProperty("COSMOSDB_ENDPOINT").ToString();
            CosmosDBMasterKey = settingValues.GetProperty("COSMOSDB_MASTER_KEY").ToString();
            CosmosDBName = settingValues.GetProperty("COSMOSDB_NAME").ToString();

            int cacheInMinutes;
            if (int.TryParse(settingValues.GetProperty("CacheInMinutes").ToString(), 
                                    out cacheInMinutes))
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
