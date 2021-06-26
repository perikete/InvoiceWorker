using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvoiceWorker.Infrastructure;
using InvoiceWorker.Infrastructure.Entities;
using Microsoft.Extensions.Logging;

namespace InvoiceWorker
{
    public class Runner
    {
        private readonly IEventFeedClient _eventFeedClient;
        private readonly IEnumerable<IEventProcessor> _eventProcessors;
        private readonly ILogger _logger;

        public Runner(IEventFeedClient eventFeedClient, IEnumerable<IEventProcessor> eventProcessors, ILogger<Runner> logger)
        {
            _eventFeedClient = eventFeedClient;
            _eventProcessors = eventProcessors;
            _logger = logger;
        }

        public async Task<RunResult> StartProcessingInvoices(int pageSize, int afterEventId)
        {
            var processingResult = new RunResult(afterEventId);
            var feedItems = await _eventFeedClient.GetFeedItems(pageSize, afterEventId);

            _logger.LogInformation("Starting processing of events...");

            foreach (var item in feedItems.Items)
            {
                var eventProcessor = _eventProcessors
                    .FirstOrDefault(ep => ep.CanHandle(item.Type));

                if (eventProcessor == null)
                    throw new RunnerException($"Cannot find a valid event processor for event type: {item.Type}");

                _logger.LogInformation($"Processing event id: {item.Id}");

                var result = await eventProcessor.ProcessInvoice(item.Content);

                LogResult(result, item);

                processingResult.SetResult(result, item.Id);
            }

            return processingResult;
        }

        private void LogResult(InvoiceProcessResult result, InvoiceFeedItem item)
        {
            switch (result)
            {
                case InvoiceProcessResult.Fail:
                    _logger.LogWarning($"Event Id: {item.Id} of Type: {item.Type} failed to process!");
                    break;
                case InvoiceProcessResult.Success:
                    _logger.LogInformation($"Event Id: {item.Id} of Type: {item.Type} processed successfully.");
                    break;
            }
        }
    }
}
