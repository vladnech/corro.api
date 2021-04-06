using Correpondence.Entities;
using Microsoft.Azure.Cosmos;
using SetupDatabaseAndData.Config;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SetupDatabaseAndData
{
    class Program
    {
        private static IFunctionSettings _functionSettings;
        private static CosmosClient _cosmosClient;
        private static Database _database;
        private static Container _container;
        private static string _containerName = "investor-docs";
        private static List<string> _prodNames = new List<string>() { "Fixed Term Annuity", "Liquid Lifetime Annuity", "Guarantied Lifetime Annuity" };
        private static List<string> _corroTypes = new List<string>() { "Change details", "Statement", "Maturity completeness", "Centerlink Schedule" };
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Reading configuration...");
            _functionSettings = new FunctionSettings();
            _cosmosClient = new CosmosClient(_functionSettings.CosmosDBEndpoint,
                                             _functionSettings.CosmosDBMasterKey, 
                                             new CosmosClientOptions() { 
                                                 ApplicationName = "Correspondence.Api.Setup" 
                                             });
            Console.WriteLine("Configuration for {0} is ready", _functionSettings.CosmosDBEndpoint);
            
            await CreateDatabaseAsync();
            await CreateContainerAsync();
            await AddItemsToContainerAsync();

            Console.WriteLine("Setting up of DB is finished");
            Console.ReadLine();
        }
        private static async Task CreateDatabaseAsync()
        {
            Console.WriteLine("Created Database if it does not exists: {0}\n", _functionSettings.CosmosDBName);
            _database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_functionSettings.CosmosDBName);
            Console.WriteLine("Created Database: {0}\n", _database.Id);
        }
        private static async Task CreateContainerAsync()
        {
            Console.WriteLine("Created Container if it does not exists: {0}\n", _containerName);
            _container = await _database.CreateContainerIfNotExistsAsync(_containerName, "/InvestorId", 400);
            Console.WriteLine("Created Container: {0}\n", _container.Id);
        }
        private static async Task AddItemsToContainerAsync()
        {
            //await GenerateDocuments(1, 100);
            //await GenerateDocuments(2, 500);
            //await GenerateDocuments(3, 1000);
            await GenerateNestedDocuments(4, 100);
            await GenerateNestedDocuments(5, 500);
            await GenerateNestedDocuments(6, 1000);
        }
        private static async Task GenerateDocuments(int investorId, int numberOfDocs)
        {
            var startDate = new DateTime(2018, 1, 1);
            var r = new Random();
            int counter = 0;
            for (int i = 1; i <= numberOfDocs; ++i)
            {
                int j = r.Next(0, 3);
                int y = r.Next(0, 2);

                var corro = new InvestorDocumentFlat()
                {
                    InvestorId = investorId,
                    id = Guid.NewGuid(),
                    RefId = investorId.ToString() + i.ToString("D4"),
                    AccountNo = investorId.ToString() + r.Next(100, 999) + i.ToString("D4"),
                    Type = _corroTypes[j],
                    Date = startDate.AddMonths(i),
                    ProductCode = "",
                    ProductName = _prodNames[y]
                };

                try
                {
                    ItemResponse<InvestorDocumentFlat> response = await _container.CreateItemAsync(corro, new PartitionKey(corro.InvestorId));
                    ++counter;

                    if (counter%10 == 0)
                        Console.WriteLine("INFO: Inserted {0} records", counter);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: {0}", ex.Message);
                    break;
                }
            }

            Console.WriteLine("TOTAL: Inserted {0} of lines", counter);
        }
        private static async Task GenerateNestedDocuments(int investorId, int numberOfDocs)
        {
            var startDate = new DateTime(2018, 1, 1);
            var r = new Random();
            
            var doc = new InvestorDocument()
            {
                InvestorId = investorId,
                id = Guid.NewGuid(),
                Corro = new List<DocumentDetails>()
            };

            for (int i = 1; i <= numberOfDocs; ++i)
            {
                int j = r.Next(0, 3);
                int y = r.Next(0, 2);

                var corro = new DocumentDetails()
                {
                    RefId = investorId.ToString() + i.ToString("D4"),
                    AccountNo = investorId.ToString() + r.Next(100, 999) + i.ToString("D4"),
                    Type = _corroTypes[j],
                    Date = startDate.AddMonths(i),
                    ProductCode = "",
                    ProductName = _prodNames[y]
                };
                doc.Corro.Add(corro);
            }

            try
            {
                ItemResponse<InvestorDocument> response = await _container.CreateItemAsync(doc, new PartitionKey(doc.InvestorId));
                Console.WriteLine("INFO: Investor documents {0} are registered", doc.Corro.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: {0}", ex.Message);
            }
        }
    } 
}
