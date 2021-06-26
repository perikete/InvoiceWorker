using System.Collections.Generic;
using InvoiceWorker.Infrastructure;

namespace InvoiceWorker
{
    /// <summary>
    /// Results for Invoice Processing run.
    /// </summary>
    public class RunResult
    {
        /// <summary>
        /// Gets the last processed event identifier.
        /// </summary>
        public int LastProcessedId { get; private set; }
        /// <summary>
        /// Gets the process results.
        /// </summary>
        public IDictionary<InvoiceProcessResult, int> ProcessResults { get; }

        public RunResult(int lastProcessedId)
        {
            ProcessResults = new Dictionary<InvoiceProcessResult, int>
            {
                {InvoiceProcessResult.Fail, 0},
                {InvoiceProcessResult.Success, 0}
            };

            LastProcessedId = lastProcessedId;
        }

        public void SetResult(InvoiceProcessResult result, int lastProcessedId)
        {
            ProcessResults[result] += 1;
            LastProcessedId = lastProcessedId;
        }
    }
}
