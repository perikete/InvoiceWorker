using System;
using InvoiceWorker.EventProcessors.PdfExporter;
using InvoiceWorker.EventProcessors.Processors;
using InvoiceWorker.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceWorker.EventProcessors
{
    public static class ServiceCollectionExtensions
    {
        private const string InvoicePdfDirectoryParam = "invoice-dir";
        public static IServiceCollection AddEventProcessors(this IServiceCollection services, IConfiguration configuration)
        {
            var invoiceDirectory = configuration[InvoicePdfDirectoryParam];

            if (string.IsNullOrWhiteSpace(invoiceDirectory))
                throw new ArgumentNullException($"{InvoicePdfDirectoryParam} parameter needs to be specified.");

            services.Configure<InvoiceLocationOptions>(o => o.BaseDirectory = invoiceDirectory);

            RegisterEventProcessors(services);

            services.AddSingleton<IInvoiceExporter, PdfInvoiceExporter>();

            return services;
        }

        private static void RegisterEventProcessors(IServiceCollection services)
        {
            services.AddSingleton<IEventProcessor, CreateInvoiceEventProcessor>()
                .AddSingleton<IEventProcessor, UpdateInvoiceEventProcessor>()
                .AddSingleton<IEventProcessor, DeleteInvoiceEventProcessor>();
        }
    }
}
