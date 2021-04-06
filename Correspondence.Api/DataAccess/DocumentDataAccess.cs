using Challenger.API.Shared;
using Correpondence.Entities;
using Correspondence.Api.Config;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Correspondence.Api.DataAccess
{
    public class DocumentDataAccess : IDocumentDataAccess
    {
        private IFunctionSettings _functionSettings;
        private IMemoryCache _cache;
        public DocumentDataAccess(IFunctionSettings functionSettings, IMemoryCache memoryCache) 
        {
            _functionSettings = functionSettings;
            _cache = memoryCache;
        }
        public async Task<PagedList<DocumentDetails>> GetPagedCorrespondenceByInvestorId(int investorId, int pageNumber = 1, int pageSize = 10)
        {
            var cacheKey = string.Format($"customer_corro_{investorId}");

            if (_cache.Get(cacheKey) != null)
            {
                var list = _cache.Get(cacheKey) as List<DocumentDetails>;
                if (list != null) {
                    return new PagedList<DocumentDetails>(list.AsQueryable(), pageNumber, pageSize);
                }
            }

            var result = new List<DocumentDetails>();
            using (var client = new CosmosClient(_functionSettings.CosmosDBEndpoint, _functionSettings.CosmosDBMasterKey))
            {
                var container = client.GetContainer(_functionSettings.CosmosDBName,
                                                    SQLStatements.InvestorDocuments.ContainerName);
                var sql = SQLStatements.InvestorDocuments.ListInvestorDocs;
                var query = new QueryDefinition(sql);
                
                query.WithParameter("@InvestorId", investorId);
                var iterator = container
                                    .GetItemQueryIterator<InvestorDocument>(query, 
                                        requestOptions: new QueryRequestOptions() { 
                                                                MaxConcurrency = 1
                                                              });

                while (iterator.HasMoreResults) 
                {
                    var task = await iterator.ReadNextAsync();
                    FeedResponse<InvestorDocument> response = task;
                    if (response.Any())
                    {
                        foreach(var item in response)
                            result.AddRange(item.Corro);
                        
                        var expireTime = DateTimeOffset.Now.AddMinutes(_functionSettings.CacheInMinutes); 
                        _cache.Set(cacheKey, result, expireTime);
                    }
                }
            }

            return new PagedList<DocumentDetails>(result.AsQueryable(), pageNumber, pageSize);
        }

        public async Task<Guid> InsertUpdateDocument(InvestorDocumentFlat document)
        {
            using (var client = new CosmosClient(_functionSettings.CosmosDBEndpoint, _functionSettings.CosmosDBMasterKey))
            {
                var container = client.GetContainer(_functionSettings.CosmosDBName,
                                                    SQLStatements.InvestorDocuments.ContainerName);
                var sql = SQLStatements.InvestorDocuments.ListInvestorDocs;
                var query = new QueryDefinition(sql);

                query.WithParameter("@InvestorId", document.InvestorId);
                var iterator = container
                                    .GetItemQueryIterator<InvestorDocument>(query,
                                        requestOptions: new QueryRequestOptions()
                                        {
                                            MaxConcurrency = 1
                                        });

                var docDetails = new DocumentDetails()
                {
                    RefId = document.RefId,
                    Type = document.Type,
                    AccountNo = document.AccountNo,
                    Date = document.Date,
                    ProductCode = document.ProductCode,
                    ProductName = document.ProductName,
                    Link = document.Link
                };

                while (iterator.HasMoreResults)
                {
                    var task = iterator.ReadNextAsync();
                    task.Wait();
                    FeedResponse<InvestorDocument> response = task.Result;
                    if (response.Any())
                    {
                        var item = response.FirstOrDefault();
                        if (item != null)
                        {
                            var d = item.Corro.Where(x => x.RefId == document.RefId).FirstOrDefault();

                            if (d == null)
                                item.Corro.Add(docDetails);
                            else
                                //Only update the link to document 
                                d.Link = document.Link;
                            
                            await container.ReplaceItemAsync(item, item.id.ToString(), new PartitionKey(item.InvestorId));
                            return item.id;                            
                        }
                    }
                    else
                        break;
                }

                var doc = new InvestorDocument()
                {
                    InvestorId = document.InvestorId,
                    id = Guid.NewGuid(),
                    Corro = new List<DocumentDetails>()
                };

                doc.Corro.Add(docDetails);

                var insertResponse = await container.CreateItemAsync(doc, new PartitionKey(doc.InvestorId));
                document.id = insertResponse.Resource.id;
            }

            return document.id;
        }
    }
}
