using System.Threading.Tasks;
using InvoiceWorker.Infrastructure.Entities;

namespace InvoiceWorker.Infrastructure
{
    public interface IEventFeedClient
    {
        /// <summary>
        /// Gets the feed items.
        /// </summary>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="afterEventId">The after event identifier.</param>
        Task<InvoiceFeedData> GetFeedItems(int pageSize = 10, int afterEventId = 0);
    }
}