namespace Correspondence.Api.Config
{
    public interface IFunctionSettings
    {
        string CosmosDBEndpoint { get; }
        string CosmosDBMasterKey { get; }
        string CosmosDBName { get; }
        int CacheInMinutes { get; }
    }
}
