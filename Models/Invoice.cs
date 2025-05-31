using System;

namespace AWEElectronics.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        public decimal Subtotal { get; set; }

        public decimal Tax { get; set; }

        public decimal Total => Subtotal + Tax;

        public string BillingAddress { get; set; }
    }

}
