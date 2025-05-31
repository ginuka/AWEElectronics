using System.Collections.Generic;
using System;
using System.Linq;

namespace AWEElectronics.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public string Status { get; set; } // e.g., "Pending", "Shipped", "Delivered"

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        public Invoice Invoice { get; set; }

        //public Payment Payment { get; set; }

        //public Shipment Shipment { get; set; }

        public decimal TotalAmount => Items?.Sum(i => i.Quantity * i.UnitPrice) ?? 0;
    }

    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }

}
