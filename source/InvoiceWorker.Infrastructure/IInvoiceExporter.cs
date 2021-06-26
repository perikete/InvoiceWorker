using System.Threading.Tasks;

namespace InvoiceWorker.Infrastructure
{
    /// <summary>
    /// An Invoice Exporter interface
    /// </summary>
    public interface IInvoiceExporter
    {
        /// <summary>
        /// Exports the given contents into the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="contentToExport">The content to export.</param>
        Task Export(string filename, string contentToExport);
    }
}
