using System.Collections.Generic;

namespace InvoiceWorker.Infrastructure.Entities
{
    public class InvoiceFeedData
    {
        public IEnumerable<InvoiceFeedItem> Items { get; set; }
    }
}
