using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using InvoiceWorker.Infrastructure;
using InvoiceWorker.Infrastructure.Entities;

namespace InvoiceWorker.EventFeedClient
{
    /// <summary>
    /// A HTTP Event Feed client implementation
    /// </summary>
    public class EventFeedClient : IEventFeedClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public EventFeedClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

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
            if (pageSize < 0 || pageSize > 500)
                throw new ArgumentOutOfRangeException(nameof(pageSize));

            var response =
                await _httpClient.GetAsync($"invoices/events?pageSize={pageSize}&afterEventId={afterEventId}");

            response.EnsureSuccessStatusCode();

            await using var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<InvoiceFeedData>(responseStream, _jsonSerializerOptions);
        }
    }
}
