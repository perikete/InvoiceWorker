using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvoiceWorker.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InvoiceWorker
{
    class Program
    {
        private static readonly IEnumerable<string> RequiredCommandLineArgs = new[] { "--feed-url", "--invoice-dir" };
        private const int DelayToRefetchInMs = 4000; // This could be moved to a configuration file/command line parameter.
        private const int PageSize = 10; // This could be moved to a configuration file/command line parameter.
        
        static async Task Main(string[] args)
        {

            if (!ValidateCommandLineParams(args))
            {
                PrintBanner();
                return;
            }

            using var host = CreateHostBuilder(args).Build();
            await Run(host.Services);
            await host.RunAsync();
        }

        private static void PrintBanner()
        {
            Console.WriteLine($"usage: {AppDomain.CurrentDomain.FriendlyName} --feed-url=<http_url> --invoice-dir=<directory_path>");
        }

        private static bool ValidateCommandLineParams(string[] args)
        {
            return args.Length >= RequiredCommandLineArgs.Count() &&
                   RequiredCommandLineArgs.All(arg => args.Any(o => o.StartsWith(arg)));
        }


        private static async Task Run(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var logger = services.GetRequiredService<ILogger<Program>>();
            var stateService = services.GetRequiredService<IStateService>();

            var provider = scope.ServiceProvider;

            var runner = provider.GetRequiredService<Runner>();
            var lastState = await stateService.GetState();
            var lastEventId = lastState.LastEventId;

            while (true)
            {
                try
                {
                    var processResult = await runner.StartProcessingInvoices(PageSize, lastEventId);

                    lastEventId = processResult.LastProcessedId;

                    LogResults(logger, processResult);

                    await stateService.SaveState(new State { LastEventId = lastEventId });

                    await Task.Delay(DelayToRefetchInMs); // added delay to avoid putting too much pressure to the API.
                }

                catch (Exception e)
                {
                    logger.LogError(e, "Exception while processing invoices!");
                }
            }
        }

        private static void LogResults(ILogger logger, RunResult processResult)
        {
            logger.LogInformation("Invoice Processing finished. Results:");
            logger.LogInformation(
                $"{processResult.ProcessResults[InvoiceProcessResult.Success]} successful and {processResult.ProcessResults[InvoiceProcessResult.Fail]} failed.");
            logger.LogInformation($"Last processed Event Id: {processResult.LastProcessedId}");
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureAppConfiguration((_, configBuilder) => configBuilder.AddCommandLine(args))
                .ConfigureServices((builder, services) => services.RegisterInvoiceWorkerDependencies(builder));
    }
}
