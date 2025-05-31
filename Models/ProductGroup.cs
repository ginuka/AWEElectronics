using System.ComponentModel.DataAnnotations;

namespace AWEElectronics.Models
{
    public class ProductGroup
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
