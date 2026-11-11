using System.Collections.Generic;
using _25dh110430_MyStore.Models;

namespace _25dh110430_MyStore.Models.ViewModel
{
    public class ProductStatisticsVM
    {
        public int TotalProducts { get; set; }
        public int TotalStock { get; set; }
        public int LowStockProducts { get; set; }
        public int OutOfStockProducts { get; set; }

        public List<Product> Products { get; set; }

        public int PageSize { get; set; }

        public bool HasMore { get; set; }

        public List<string> BestSellingProductNames { get; set; }
        public List<int> BestSellingQuantities { get; set; }

        public ProductStatisticsVM()
        {
            Products = new List<Product>();
            PageSize = 5;
        }
    }
}