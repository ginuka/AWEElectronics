using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWEElectronics.Models
{
    public class Product
    {
        public int Id { get; set; }

        public int ProductGroupId { get; set; }
        public ProductGroup ProductGroup { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }
        public byte[] Image { get; set; }
        public int Availability { get; set; }
    }
}
