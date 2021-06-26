using System.Linq;
using System.Threading.Tasks;
using InvoiceWorker.EventFeedClient;
using InvoiceWorker.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace InvoiceWorker.Tests.EventFeedClient
{
    public class JsonFileEventFeedClientTests
    {
        private const string ValidEventJsonFileName = "EventFeedClient\\valid_invoice_events.json";

        private readonly OptionsWrapper<JsonFileFeedClientOptions> _validJsonEventFile =
            new OptionsWrapper<JsonFileFeedClientOptions>(new JsonFileFeedClientOptions
            { JsonFilename = ValidEventJsonFileName });

        [Fact]
        public async Task GetFeedItems_ValidJson_Return_Items()
        {
            var client = new JsonFileEventFeedClient(_validJsonEventFile, new NullLogger<JsonFileEventFeedClient>());

            var feedData = await client.GetFeedItems();

            Assert.Equal(10, feedData.Items.Count());
            var firstEvent = feedData.Items.First(o => o.Id == 1);
            Assert.Equal(EventType.INVOICE_CREATED, firstEvent.Type);
            Assert.Single(firstEvent.Content.LineItems);

            var secondEvent = feedData.Items.First(o => o.Id == 3);
            Assert.Equal(EventType.INVOICE_UPDATED, secondEvent.Type);
            Assert.Equal(2, secondEvent.Content.LineItems.Count());
        }
    }
}
