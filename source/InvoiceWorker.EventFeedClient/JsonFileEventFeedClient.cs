using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using InvoiceWorker.Infrastructure;
using InvoiceWorker.Infrastructure.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InvoiceWorker.EventFeedClient
{
    /// <summary>
    /// A JSON file event feed client to read feed items from a local json file.
    /// </summary>
    public class JsonFileEventFeedClient : IEventFeedClient
    {
        private readonly JsonFileFeedClientOptions _options;
        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public JsonFileEventFeedClient(IOptions<JsonFileFeedClientOptions> options, ILogger<JsonFileEventFeedClient> logger)
        {
            _options = options.Value;
            _logger = logger;

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            };
        }

        /// <inheritdoc />
        public async Task<InvoiceFeedData> GetFeedItems(int pageSize = 10, int afterEventId = 0)
        {
            _logger.LogInformation($"Reading invoice events from local file: {_options.JsonFilename}");

            await using var stream = File.OpenRead(_options.JsonFilename);
            var feedData = await JsonSerializer.DeserializeAsync<InvoiceFeedData>(stream, _jsonSerializerOptions);

            var items = feedData
                .Items
                .OrderBy(o => o.Id)
                .SkipWhile(o => o.Id < afterEventId + 1)
                .Take(pageSize);

            return new InvoiceFeedData { Items = items };
        }
    }
}
