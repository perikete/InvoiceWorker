using System;

namespace InvoiceWorker.Infrastructure.Entities
{
    public class InvoiceFeedItem
    {
        public int Id { get; set; }
        public EventType Type { get; set; }
        public InvoiceContent Content { get; set; }
        public DateTimeOffset CreatedDateUtc { get; set; }
    }
}