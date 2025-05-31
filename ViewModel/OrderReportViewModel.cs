using AWEElectronics.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace AWEElectronics.ViewModel
{
    public class OrderReportViewModel
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Period { get; set; }  // "daily", "monthly", "yearly"
        public List<Order> Orders { get; set; } = new List<Order>();

        public decimal TotalAmount => Orders.Sum(o => o.Items.Sum(i => i.Quantity * i.UnitPrice));
    }
}
