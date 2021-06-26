using System;

namespace InvoiceWorker.EventFeedClient
{
    public class InvalidJsonClientConfigurationException : Exception
    {
        public InvalidJsonClientConfigurationException(string message) 
            : base(message)
        {
        }
    }
}
