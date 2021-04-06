using System;

namespace Correpondence.Entities
{
    public class InvestorDocumentFlat: DocumentDetails
    {
        public int InvestorId { get; set; } 
        public Guid id { get; set; }
    }
}
