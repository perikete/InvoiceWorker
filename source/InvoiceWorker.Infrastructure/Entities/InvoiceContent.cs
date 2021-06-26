using System;
using System.Collections.Generic;
using System.Linq;

namespace InvoiceWorker.Infrastructure.Entities
{
    public class InvoiceContent
    {
        public Guid InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public IEnumerable<InvoiceLineItem> LineItems { get; set; }
        public InvoiceStatus Status { get; set; }
        public DateTimeOffset DueDateUtc { get; set; }
        public DateTimeOffset CreatedDateUtc { get; set; }
        public DateTimeOffset? UpdatedDateUtc { get; set; }
        public decimal TotalInvoice => LineItems.Sum(o => o.LineItemTotalCost);
    }
}