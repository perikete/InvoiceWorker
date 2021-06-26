using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceWorker.EventProcessors;
using InvoiceWorker.EventProcessors.Processors;
using InvoiceWorker.Infrastructure;
using InvoiceWorker.Infrastructure.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace InvoiceWorker.Tests.InvoiceEventProcessors
{
    public class CreateInvoiceEventProcessorTests
    {
        private readonly CreateInvoiceEventProcessor _eventProcessor;
        private readonly Mock<IInvoiceExporter> _invoiceExporter;

        public CreateInvoiceEventProcessorTests()
        {
            _invoiceExporter = new Mock<IInvoiceExporter>();
            _eventProcessor = new CreateInvoiceEventProcessor(_invoiceExporter.Object, new NullLogger<CreateInvoiceEventProcessor>());
        }

        [Fact]
        public void CanHandle_CreateEvent()
        {
            var canHandle = _eventProcessor.CanHandle(EventType.INVOICE_CREATED);

            Assert.True(canHandle);
        }

        [Fact]
        public async Task ProcessInvoice_CreatePdf()
        {
            var invoiceId = Guid.NewGuid();
            var expectedFilename = $"{invoiceId}.pdf";
            var invoiceContent = new InvoiceContent
            {
                InvoiceId = invoiceId,
                InvoiceNumber = "3332223",
                LineItems = new List<InvoiceLineItem> {new InvoiceLineItem {Description = "Item 1"}},
                CreatedDateUtc = DateTimeOffset.UtcNow
            };

            var result = await _eventProcessor.ProcessInvoice(invoiceContent);

            _invoiceExporter.Verify(o => o.Export(expectedFilename, invoiceContent.ToTemplatedString()));
            Assert.Equal(InvoiceProcessResult.Success, result);
        }
    }
}
