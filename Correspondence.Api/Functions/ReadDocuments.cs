using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Correspondence.Api.DataAccess;
using Correspondence.Api.Utils;
using Challenger.API.Shared;
using Correpondence.Entities;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;
using System;

namespace Correspondence.Api.Functions
{
    public class ReadDocuments
    {
        private IDocumentDataAccess _documentDataAccess;
        public ReadDocuments(IDocumentDataAccess documentDataAccess) 
        {
            _documentDataAccess = documentDataAccess;
        }

        [FunctionName("Documents")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Read documents" })]
        [OpenApiParameter(name: "investorId", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "OneVue Investor Id")]
        [OpenApiParameter(name: "pageNumber", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "Page number")]
        [OpenApiParameter(name: "pageSize", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "Page size")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ApiResult<PagedList<DocumentDetails>>), Description = "Return ApiResult<PagedList<DocumentDetails>>")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var result = new ApiResult<PagedList<DocumentDetails>>();

            log.LogInformation("Start get Documents...");
            
            var investorId = req.Query.GetIntValue("investorId");
            if (!investorId.HasValue)
            {
                result.Success = false;
                result.AddError("The investorId parameter is not provided or invalid format");
                return new BadRequestObjectResult(result);
            }

            var pageNumber = req.Query.GetIntValue("pageNumber");
            if (!pageNumber.HasValue)
            {
                result.Success = false;
                result.AddError("The pageNumber parameter is not valid format");
                return new BadRequestObjectResult(result);
            }

            var pageSize = req.Query.GetIntValue("pageSize");
            if (!pageSize.HasValue)
            {
                result.Success = false;
                result.AddError("The pageSize parameter is not valid format");
                return new BadRequestObjectResult(result);
            }

            try
            {
                var list = await _documentDataAccess.GetPagedCorrespondenceByInvestorId(investorId.Value, pageNumber.Value, pageSize.Value);
                result.Data = list;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.AddError(ex.Message);
                log.LogError(ex.Message);
            }
            finally
            {
                log.LogInformation("End get Documents");
            }

            return new JsonResult(result);
        }
    }
}
