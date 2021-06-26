using System;

namespace InvoiceWorker
{
    /// <summary>
    /// Exception class to signal errors with the Events Runner.
    /// </summary>
    public class RunnerException : Exception
    {
        public RunnerException(string message) : base(message)
        {
        }
    }
}
