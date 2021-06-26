using System.Threading.Tasks;
using InvoiceWorker.Infrastructure;
using InvoiceWorker.Infrastructure.Entities;
using Microsoft.Extensions.Logging;

namespace InvoiceWorker.EventProcessors.Processors
{
    /// <summary>
    /// Handles events to create new PDF invoices.
    /// </summary>
    public class CreateInvoiceEventProcessor : IEventProcessor
    {
        private readonly IInvoiceExporter _invoiceExporter;
        private readonly ILogger<CreateInvoiceEventProcessor> _logger;

        public CreateInvoiceEventProcessor(IInvoiceExporter invoiceExporter,
            ILogger<CreateInvoiceEventProcessor> logger)
        {
            _invoiceExporter = invoiceExporter;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<InvoiceProcessResult> ProcessInvoice(InvoiceContent invoice)
        {
            _logger.LogInformation($"Processing CREATE invoice for Invoice Id: {invoice.InvoiceId}");

            var invoiceFilename = $"{invoice.InvoiceId}.pdf";
            await _invoiceExporter.Export(invoiceFilename, invoice.ToTemplatedString());

            return InvoiceProcessResult.Success;
        }

        /// <inheritdoc />
        public bool CanHandle(EventType eventType) => eventType == EventType.INVOICE_CREATED;
    }
}
