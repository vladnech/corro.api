using Challenger.API.Shared;
using Correpondence.Entities;
using System;
using System.Threading.Tasks;

namespace Correspondence.Api.DataAccess
{
    public interface IDocumentDataAccess
    {
        Task<PagedList<DocumentDetails>> GetPagedCorrespondenceByInvestorId(int customerId, int pageNumber = 1, int pageSize = 10);
        Task<Guid> InsertUpdateDocument(InvestorDocumentFlat document);
    }
}
