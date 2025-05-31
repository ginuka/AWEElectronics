using System.Collections.Generic;
using System.Linq;

namespace AWEElectronics.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public ICollection<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();

        public decimal TotalAmount => Items?.Sum(i => i.Quantity * i.UnitPrice) ?? 0;
    }

    public class ShoppingCartItem
    {
        public int Id { get; set; }

        public int ShoppingCartId { get; set; }
        public ShoppingCart ShoppingCart { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }

}
