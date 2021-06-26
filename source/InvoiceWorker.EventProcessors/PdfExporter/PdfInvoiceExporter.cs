using System.IO;
using System.Threading.Tasks;
using InvoiceWorker.Infrastructure;
using IronPdf;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InvoiceWorker.EventProcessors.PdfExporter
{
    /// <summary>
    /// A PDF invoice exporter implementation.
    /// </summary>
    public class PdfInvoiceExporter : IInvoiceExporter
    {
        private readonly ILogger<PdfInvoiceExporter> _logger;
        private readonly InvoiceLocationOptions _options;

        public PdfInvoiceExporter(IOptions<InvoiceLocationOptions> options, ILogger<PdfInvoiceExporter> logger)
        {
            _logger = logger;
            _options = options.Value;
        }

        /// <inheritdoc />
        public async Task Export(string filename, string contentToExport)
        {
            if (!Directory.Exists(_options.BaseDirectory))
                throw new DirectoryNotFoundException($"The PDF directory: {_options.BaseDirectory} does not exists!");

            var filePath = Path.Combine(_options.BaseDirectory, filename);
            var htmlToPdf = new HtmlToPdf();
            var pdf = await htmlToPdf.RenderHtmlAsPdfAsync(contentToExport);

            pdf.SaveAs(filePath);

            _logger.LogInformation($"PDF file generated: {filename}");
        }
    }
}
