using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceWorker.Infrastructure;
using InvoiceWorker.Infrastructure.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace InvoiceWorker.Tests
{
    public class RunnerTests
    {
        private readonly Runner _runner;
        private readonly List<InvoiceFeedItem> _events;
        private readonly Mock<IEventProcessor> _updateEventProcessor;
        private readonly Mock<IEventProcessor> _createEventProcessor;
        private readonly InvoiceContent _createInvoiceContent;
        private readonly InvoiceContent _updateInvoiceContent;

        public RunnerTests()
        {
            _createInvoiceContent = new InvoiceContent { InvoiceId = Guid.NewGuid() };
            _updateInvoiceContent = new InvoiceContent { InvoiceId = Guid.NewGuid() };
            _events = new List<InvoiceFeedItem>
            {
                new InvoiceFeedItem {Id = 1, Type = EventType.INVOICE_CREATED, Content = _createInvoiceContent},
                new InvoiceFeedItem {Id = 2, Type = EventType.INVOICE_UPDATED, Content = _updateInvoiceContent}
            };

            var feedClient = new Mock<IEventFeedClient>();
            feedClient.Setup(o => o.GetFeedItems(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new InvoiceFeedData { Items = _events });

            _updateEventProcessor = new Mock<IEventProcessor>();
            _updateEventProcessor
                .Setup(o => o.CanHandle(It.Is<EventType>(ev => ev == EventType.INVOICE_UPDATED)))
                .Returns(true);

            _createEventProcessor = new Mock<IEventProcessor>();
            _createEventProcessor
                .Setup(o => o.CanHandle(It.Is<EventType>(ev => ev == EventType.INVOICE_CREATED)))
                .Returns(true);

            IEnumerable<IEventProcessor> eventsProcessors = new List<IEventProcessor> { _updateEventProcessor.Object, _createEventProcessor.Object };

            _runner = new Runner(feedClient.Object, eventsProcessors, new NullLogger<Runner>());
        }

        [Fact]
        public async Task StartProcessingInvoices_ProcessEvents_Success()
        {
            await _runner.StartProcessingInvoices(10, 0);

            _createEventProcessor.Verify(o => o.ProcessInvoice(It.Is<InvoiceContent>(i => i.InvoiceId == _createInvoiceContent.InvoiceId)), Times.Once);
            _updateEventProcessor.Verify(o => o.ProcessInvoice(It.Is<InvoiceContent>(i => i.InvoiceId == _updateInvoiceContent.InvoiceId)), Times.Once);
        }

        [Fact]
        public async Task StartProcessingInvoices_ProcessEvent_With_No_Processor_Should_Throw()
        {
            _events.Add(new InvoiceFeedItem { Type = EventType.INVOICE_DELETED });

            var ex = await Assert.ThrowsAsync<RunnerException>(() => _runner.StartProcessingInvoices(10, 0));

            Assert.Equal("Cannot find a valid event processor for event type: INVOICE_DELETED", ex.Message);
        }
    }
}
