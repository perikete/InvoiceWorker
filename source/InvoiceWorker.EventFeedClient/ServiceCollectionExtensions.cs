using System;
using System.IO;
using InvoiceWorker.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceWorker.EventFeedClient
{
    public static class ServiceCollectionExtensions
    {
        private const string LocalJsonFilenameParam = "local-json";
        private const string FeedUrlParam = "feed-url";
        public static IServiceCollection AddInvoiceJsonClient(this IServiceCollection services,
            IConfiguration configuration)
        {
            var localJsonFilename = configuration[LocalJsonFilenameParam];

            if (localJsonFilename != null)
            {
                if (!File.Exists(localJsonFilename))
                    throw new InvalidJsonClientConfigurationException(
                        $"Local JSON file: {localJsonFilename} not found!");

                services
                    .Configure<JsonFileFeedClientOptions>(o => o.JsonFilename = localJsonFilename)
                    .AddSingleton<IEventFeedClient, JsonFileEventFeedClient>();

            }
            else if (configuration[FeedUrlParam] != null && Uri.TryCreate(configuration[FeedUrlParam], UriKind.Absolute, out var feedUri))
            {
                services.AddHttpClient<IEventFeedClient, EventFeedClient>(c =>
                {
                    c.BaseAddress = feedUri;
                });
            }

            return services;
        }
    }
}
