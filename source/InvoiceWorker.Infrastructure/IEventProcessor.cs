using System.Threading.Tasks;
using InvoiceWorker.Infrastructure.Entities;

namespace InvoiceWorker.Infrastructure
{
    /// <summary>
    /// Interface for processing feed events.
    /// </summary>
    public interface IEventProcessor
    {
        /// <summary>
        /// Processes the given invoice.
        /// </summary>
        /// <param name="invoice">The invoice to process.</param>
        Task<InvoiceProcessResult> ProcessInvoice(InvoiceContent invoice);
        /// <summary>
        /// Determines whether this instance can handle the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        bool CanHandle(EventType eventType);
    }
}   