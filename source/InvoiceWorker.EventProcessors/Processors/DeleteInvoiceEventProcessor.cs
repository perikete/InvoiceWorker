using System.IO;
using System.Threading.Tasks;
using InvoiceWorker.Infrastructure;
using InvoiceWorker.Infrastructure.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InvoiceWorker.EventProcessors.Processors
{
    /// <summary>
    /// Handles events to delete invoices from the filesystem.
    /// </summary>
    public class DeleteInvoiceEventProcessor : IEventProcessor
    {
        private readonly ILogger<DeleteInvoiceEventProcessor> _logger;
        private readonly InvoiceLocationOptions _options;

        public DeleteInvoiceEventProcessor(IOptions<InvoiceLocationOptions> options, ILogger<DeleteInvoiceEventProcessor> logger)
        {
            _logger = logger;
            _options = options.Value;
        }

        /// <inheritdoc />
        public async Task<InvoiceProcessResult> ProcessInvoice(InvoiceContent invoice)
        {
            _logger.LogInformation($"Processing DELETE event for Invoice Id: {invoice.InvoiceId}");

            var filename = $"{invoice.InvoiceId}.pdf";
            var filePath = Path.Combine(_options.BaseDirectory, filename);

            if (!File.Exists(filePath))
            {
                _logger.LogWarning(
                    $"Cannot delete Invoice Id: {invoice.InvoiceId} with path: {filePath} because the file don't exists!. Skipping event.");
                return InvoiceProcessResult.Fail;
            }

            File.Delete(filePath);
            _logger.LogInformation($"Invoice Id: {invoice.InvoiceId} deleted.");

            return await Task.FromResult(InvoiceProcessResult.Success);
        }

        /// <inheritdoc />
        public bool CanHandle(EventType eventType) => eventType == EventType.INVOICE_DELETED;
    }
}
