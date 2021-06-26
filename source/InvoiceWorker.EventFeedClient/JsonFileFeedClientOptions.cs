namespace InvoiceWorker.EventFeedClient
{
    /// <summary>
    /// Options class for the local JSON file Event Feed client.
    /// </summary>
    public class JsonFileFeedClientOptions
    {
        /// <summary>
        /// Gets or sets the local json filename to read events from.
        /// </summary>
        public string JsonFilename { get; set; }
    }
}
