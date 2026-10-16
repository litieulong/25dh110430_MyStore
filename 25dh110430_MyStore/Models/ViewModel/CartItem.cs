using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _25dh110430_MyStore.Models.ViewModel
{
    public class CartItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string ProductImage { get; set; }
        public string Category { get; set; } // Bổ sung để lưu Tên danh mục của sản phẩm[cite: 10]

        // Tổng giá cho mỗi sản phẩm
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}