using System;
using System.IO;
using System.Threading.Tasks;

namespace InvoiceWorker.Tests
{
    public static class FileHelpers
    {
        public static async Task<string> CreateTestFile(string baseDirectory, Guid invoiceId)
        {
            var filePath = Path.Combine(baseDirectory, $"{invoiceId}.pdf");
            Directory.CreateDirectory(baseDirectory);
            await File.WriteAllTextAsync(filePath, "Test PDF file");
            return filePath;
        }
    }
}
