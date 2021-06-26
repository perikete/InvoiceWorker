using System;

namespace InvoiceWorker.EventFeedClient
{
    /// <summary>
    /// Exception class to signal configuration errors with the local JSON file client.
    /// </summary>
    public class InvalidJsonClientConfigurationException : Exception
    {
        public InvalidJsonClientConfigurationException(string message) 
            : base(message)
        {
        }
    }
}
