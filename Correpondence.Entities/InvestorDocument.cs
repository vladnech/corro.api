using System;
using System.Collections.Generic;

namespace Correpondence.Entities
{
    public class InvestorDocument   
    {
        public int InvestorId { get; set; }
        public Guid id { get; set; }
        public List<DocumentDetails> Corro { get; set; }
    }
}
