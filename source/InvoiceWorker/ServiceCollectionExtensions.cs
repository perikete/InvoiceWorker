using InvoiceWorker.EventFeedClient;
using InvoiceWorker.EventProcessors;
using InvoiceWorker.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace InvoiceWorker
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterInvoiceWorkerDependencies(this IServiceCollection services, HostBuilderContext builder)
        {
            return services
                .AddEventProcessors(builder.Configuration)
                .AddSingleton<Runner>()
                .AddInvoiceJsonClient(builder.Configuration)
                .AddSingleton<IStateService, JsonStateService>();
        }
    }
}
