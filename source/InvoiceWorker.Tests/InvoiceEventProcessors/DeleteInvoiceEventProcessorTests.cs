using System;
using System.IO;
using System.Threading.Tasks;
using InvoiceWorker.EventProcessors;
using InvoiceWorker.EventProcessors.Processors;
using InvoiceWorker.Infrastructure;
using InvoiceWorker.Infrastructure.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace InvoiceWorker.Tests.InvoiceEventProcessors
{
    public class DeleteInvoiceEventProcessorTests
    {
        private readonly DeleteInvoiceEventProcessor _eventProcessor;
        private const string InvoicesDirectory = "Invoices";

        public DeleteInvoiceEventProcessorTests()
        {
            _eventProcessor = new DeleteInvoiceEventProcessor(
                new OptionsWrapper<InvoiceLocationOptions>(new InvoiceLocationOptions
                { BaseDirectory = InvoicesDirectory }), new NullLogger<DeleteInvoiceEventProcessor>());
        }

        [Fact]
        public void CanHandle_DeleteEvent()
        {
            var canHandle = _eventProcessor.CanHandle(EventType.INVOICE_DELETED);

            Assert.True(canHandle);
        }

        [Fact]
        public async Task ProcessInvoice_FileExists_DeletePdf()
        {
            var invoiceId = Guid.NewGuid();
            var filePath = await FileHelpers.CreateTestFile(InvoicesDirectory, invoiceId);
            Assert.True(File.Exists(filePath));
            var invoiceContent = new InvoiceContent
            {
                InvoiceId = invoiceId
            };

            await _eventProcessor.ProcessInvoice(invoiceContent);

            Assert.False(File.Exists(filePath));
        }

        [Fact]
        public async Task ProcessInvoice_FileNotExists_Fail()
        {
            var invoiceId = Guid.NewGuid();
            var invoiceContent = new InvoiceContent
            {
                InvoiceId = invoiceId
            };

            var result = await _eventProcessor.ProcessInvoice(invoiceContent);

            Assert.Equal(InvoiceProcessResult.Fail, result);
        }
    }
}
