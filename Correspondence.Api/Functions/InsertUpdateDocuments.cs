using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Challenger.API.Shared;
using Correpondence.Entities;
using Correspondence.Api.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Correspondence.Api.Functions
{
    public class InsertUpdateDocuments
    {
        private IDocumentDataAccess _documentDataAccess;
        public InsertUpdateDocuments(IDocumentDataAccess documentDataAccess)
        {
            _documentDataAccess = documentDataAccess;
        }

        [FunctionName("Document")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Insert/Update Document" })]
        //[OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(InvestorDocumentFlat), Description = "Document to be added/updated")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "OK Guid")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Start insert/update function...");

            var result = new ApiResult<Guid>();

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var doc = JsonConvert.DeserializeObject<InvestorDocumentFlat>(requestBody);

            if (doc == null) 
            {
                result.Success = false;
                result.AddError("The required object of document is not provided or invalid format");
            }
            else
            {
                try
                {
                    result.Data = await _documentDataAccess.InsertUpdateDocument(doc);
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.AddError(ex.Message);
                    log.LogError(ex.Message);
                }
                finally
                {
                    log.LogInformation("End insert/update function");
                }
            }

            return new JsonResult(result);
        }
    }
}

