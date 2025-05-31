using System.Collections.Generic;
using System;
using System.Linq;

public class OrderInvoiceViewModel
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }

    public List<OrderItemViewModel> Items { get; set; }
    public decimal TotalAmount => Items.Sum(i => i.Quantity * i.UnitPrice);
}

public class OrderItemViewModel
{
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal => Quantity * UnitPrice;
}
