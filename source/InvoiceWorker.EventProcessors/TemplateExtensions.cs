using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using InvoiceWorker.Infrastructure.Entities;

namespace InvoiceWorker.EventProcessors
{
    /// <summary>
    /// Templating extension methods.
    /// </summary>
    public static class TemplateExtensions
    {
        private const string PdfTemplateFile = "PdfTemplate.html";

        /// <summary>
        /// Converts invoice content to a templated string.
        /// </summary>
        /// <param name="invoiceContent">Content of the invoice.</param>
        /// <returns></returns>
        public static string ToTemplatedString(this InvoiceContent invoiceContent)
        {
            if (!File.Exists(PdfTemplateFile))
                throw new FileNotFoundException("PDF Template not found!");

            // Pretty basic templating, this could be improved using Razor as template engine.
            var template = File.ReadAllText(PdfTemplateFile);

            var parsedTemplate = template
                .Replace("{@CreatedDateUtc}", invoiceContent.CreatedDateUtc.ToString("d"))
                .Replace("{@InvoiceNumber}", invoiceContent.InvoiceNumber)
                .Replace("{@DueDateUtc}", invoiceContent.DueDateUtc.ToString("d"))
                .Replace("{@Status}", invoiceContent.Status.ToString())
                .Replace("{@LineItems}", GetLineItemsTemplate(invoiceContent.LineItems.ToList()))
                .Replace("{@TotalInvoice}", Convert.ToString(invoiceContent.TotalInvoice, CultureInfo.CurrentCulture))
                .Replace("{@InvoiceUpdatedData}",
                    $"<h3>{invoiceContent.UpdatedDateUtc?.ToString("d") ?? string.Empty}");

            return parsedTemplate;
        }

        private static string GetLineItemsTemplate(IList<InvoiceLineItem> lineItems)
        {
            if (!lineItems.Any())
                return "<tr><td /><td /><td /><td /></tr>";

            var sb = new StringBuilder();

            foreach (var lineItem in lineItems)
            {
                sb.Append("<tr>");

                sb.Append($"<td>{lineItem.Description}</td>");
                sb.Append($"<td>{lineItem.Quantity}</td>");
                sb.Append($"<td>{lineItem.UnitCost}</td>");
                sb.Append($"<td>{lineItem.LineItemTotalCost}</td>");

                sb.Append("</tr>");
            }

            return sb.ToString();
        }
    }
}
