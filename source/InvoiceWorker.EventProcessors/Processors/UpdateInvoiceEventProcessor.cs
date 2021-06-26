using System.IO;
using System.Threading.Tasks;
using InvoiceWorker.Infrastructure;
using InvoiceWorker.Infrastructure.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InvoiceWorker.EventProcessors.Processors
{
    /// <summary>
    /// Handles events to update PDF invoices.
    /// </summary>
    public class UpdateInvoiceEventProcessor : IEventProcessor
    {
        private readonly IInvoiceExporter _invoiceExporter;
        private readonly ILogger<UpdateInvoiceEventProcessor> _logger;
        private readonly InvoiceLocationOptions _options;

        public UpdateInvoiceEventProcessor(IInvoiceExporter invoiceExporter, 
            IOptions<InvoiceLocationOptions> options,
            ILogger<UpdateInvoiceEventProcessor> logger)
        {
            _invoiceExporter = invoiceExporter;
            _options = options.Value;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<InvoiceProcessResult> ProcessInvoice(InvoiceContent invoice)
        {
            _logger.LogInformation($"Processing UPDATE invoice for Invoice Id: {invoice.InvoiceId}");

            var filename = $"{invoice.InvoiceId}.pdf";
            var filePath = Path.Combine(_options.BaseDirectory, filename);

            if (!File.Exists(filePath))
            {
                _logger.LogWarning(
                    $"Cannot update Invoice Id: {invoice.InvoiceId} with path: {filePath} because the original file don't exists!. Skipping event.");
                return InvoiceProcessResult.Fail;
            }

            var invoiceFilename = $"{invoice.InvoiceId}.pdf";
            await _invoiceExporter.Export(invoiceFilename, invoice.ToTemplatedString());

            return InvoiceProcessResult.Success;
        }

        /// <inheritdoc />
        public bool CanHandle(EventType eventType) => eventType == EventType.INVOICE_UPDATED;
    }
}
