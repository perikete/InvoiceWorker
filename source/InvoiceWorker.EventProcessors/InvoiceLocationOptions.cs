namespace InvoiceWorker.EventProcessors
{
    /// <summary>
    /// Options class for the invoices location.
    /// </summary>
    public class InvoiceLocationOptions
    {
        /// <summary>
        /// Gets or sets the base directory for the exported files.
        /// </summary>
        public string BaseDirectory { get; set; }
    }
}
