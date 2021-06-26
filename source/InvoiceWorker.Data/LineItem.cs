using System;

namespace InvoiceWorker.Data
{
    public class LineItem
    {
        public Guid LineItemId { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal LineItemTotalCost { get; set; }
    }
}