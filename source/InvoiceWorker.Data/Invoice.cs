using System;
using System.Collections.Generic;

namespace InvoiceWorker.Data
{
    public class Invoice
    {
        public Guid InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public IEnumerable<LineItem> LineItems { get; set; }
        public InvoiceStatus Status { get; set; }
        public DateTimeOffset DueDateUtc { get; set; }
        public DateTimeOffset CreatedDateUtc { get; set; }
        public DateTimeOffset UpdatedDateUtc { get; set; }
    }
}
