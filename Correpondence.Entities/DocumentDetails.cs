using System;

namespace Correpondence.Entities
{
    public class DocumentDetails
    {
        public string RefId { get; set; }
        public string Type { get; set; }
        public string AccountNo { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public DateTime Date { get; set; }
        public string Link { get; set; }
    }
}
