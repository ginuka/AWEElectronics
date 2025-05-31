using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWEElectronics.ViewModel
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public IFormFile ImageFile { get; set; }
        public IFormFile Image { get; set; }
        public byte[] ImageBytes { get; set; }
        public int Availability { get; set; }
        public int ProductGroupId { get; set; }
        public string ProductGroupName { get; set; }
        public string Group { get; set; }

        public List<SelectListItem> ProductGroups { get; set; }

        //public List<SelectListItem> ProductGroupList { get; set; } = new List<SelectListItem>();
    }
}
