using System.Collections.Generic;

namespace _25dh110430_MyStore.Models.ViewModel
{
    public class ProductColorVM
    {
        public string ColorName { get; set; }

        public List<string> Images { get; set; }

        public ProductColorVM()
        {
            Images = new List<string>();
        }
    }

    public class ProductCreateVM
    {
        public int ProductID { get; set; }

        public int CategoryID { get; set; }

        public string ProductName { get; set; }

        public decimal ProductPrice { get; set; }

        public string ProductDescription { get; set; }

        public List<ProductColorVM> Colors { get; set; }

        public ProductCreateVM()
        {
            Colors = new List<ProductColorVM>();
        }
    }
}