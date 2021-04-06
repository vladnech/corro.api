using Correspondence.Api.Config;
using Correspondence.Api.DataAccess;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

[assembly: FunctionsStartup(typeof(Correspondence.Api.Startup))]
namespace Correspondence.Api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var context = builder.Services
                                .BuildServiceProvider()
                                .GetService<IOptions<ExecutionContextOptions>>()
                                .Value;

            builder.Services.AddSingleton<IFunctionSettings>(x => new FunctionSettings(context));
            builder.Services.TryAddScoped<IDocumentDataAccess, DocumentDataAccess>();
            
        }
    }
}
