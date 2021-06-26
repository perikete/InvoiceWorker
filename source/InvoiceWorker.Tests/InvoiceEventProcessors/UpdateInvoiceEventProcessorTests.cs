using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceWorker.EventProcessors;
using InvoiceWorker.EventProcessors.Processors;
using InvoiceWorker.Infrastructure;
using InvoiceWorker.Infrastructure.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace InvoiceWorker.Tests.InvoiceEventProcessors
{
    public class UpdateInvoiceEventProcessorTests
    {
        private readonly UpdateInvoiceEventProcessor _eventProcessor;
        private readonly Mock<IInvoiceExporter> _invoiceExporter;
        private const string InvoicesDirectory = "Invoices";

        public UpdateInvoiceEventProcessorTests()
        {
            _invoiceExporter = new Mock<IInvoiceExporter>();
            _eventProcessor = new UpdateInvoiceEventProcessor(_invoiceExporter.Object,
                new OptionsWrapper<InvoiceLocationOptions>(new InvoiceLocationOptions { BaseDirectory = InvoicesDirectory }),
                new NullLogger<UpdateInvoiceEventProcessor>());
        }

        [Fact]
        public void CanHandle_UpdateEvent()
        {
            var canHandle = _eventProcessor.CanHandle(EventType.INVOICE_UPDATED);

            Assert.True(canHandle);
        }

        [Fact]
        public async Task ProcessInvoice_OriginalFile_Exists_UpdatePdf()
        {
            var invoiceId = Guid.NewGuid();
            await FileHelpers.CreateTestFile(InvoicesDirectory, invoiceId);
            var invoiceContent = new InvoiceContent
            {
                InvoiceId = invoiceId,
                InvoiceNumber = "3332223",
                LineItems = new List<InvoiceLineItem> { new InvoiceLineItem { Description = "Item 1" } },
                UpdatedDateUtc = DateTimeOffset.UtcNow
            };

            var result = await _eventProcessor.ProcessInvoice(invoiceContent);

            _invoiceExporter.Verify(o => o.Export($"{invoiceId}.pdf", invoiceContent.ToTemplatedString()));
            Assert.Equal(InvoiceProcessResult.Success, result);
        }

        [Fact]
        public async Task ProcessInvoice_OriginalFile_DontExists_Fail()
        {
            var invoiceId = Guid.NewGuid();
            var invoiceContent = new InvoiceContent
            {
                InvoiceId = invoiceId,
                InvoiceNumber = "3332223",
                LineItems = new List<InvoiceLineItem> { new InvoiceLineItem { Description = "Item 1" } },
                UpdatedDateUtc = DateTimeOffset.UtcNow
            };

            var result = await _eventProcessor.ProcessInvoice(invoiceContent);

            Assert.Equal(InvoiceProcessResult.Fail, result);
        }
    }
}
