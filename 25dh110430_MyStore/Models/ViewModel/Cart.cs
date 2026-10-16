using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using _25dh110430_MyStore.Models.ViewModel;

namespace _25dh110430_MyStore.Models.ViewModel
{
    public class Cart
    {
        private List<CartItem> items = new List<CartItem>();
        public IEnumerable<CartItem> Items => items;

        // Danh sách các sản phẩm trong giỏ hàng đã được nhóm theo Category[cite: 10]
        public List<IGrouping<string, CartItem>> GroupedItems => items.GroupBy(i => i.Category).ToList();

        // Các thuộc tính hỗ trợ tính phân trang[cite: 10]
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 6; // Số sản phẩm mỗi trang[cite: 10]

        // Danh sách các sản phẩm cùng danh mục với các sản phẩm trong giỏ[cite: 10]
        public PagedList.IPagedList<Product> SimilarProducts { get; set; }

        // Thêm sản phẩm vào giỏ
        public void AddItem(int productId, string productImg, string productName, decimal unitPrice, int quantity, string category)
        {
            var existingItem = items.FirstOrDefault(i => i.ProductID == productId);
            if (existingItem == null)
            {
                items.Add(new CartItem { ProductID = productId, ProductImage = productImg, ProductName = productName, UnitPrice = unitPrice, Quantity = quantity, Category = category });
            }
            else
            {
                existingItem.Quantity += quantity;
            }
        }

        // Xóa sản phẩm khỏi giỏ
        public void RemoveItem(int productId)
        {
            items.RemoveAll(i => i.ProductID == productId);
        }

        // Tính tổng giá trị giỏ hàng
        public decimal TotalValue()
        {
            return items.Sum(i => i.TotalPrice);
        }

        // Làm trống giỏ hàng
        public void Clear()
        {
            items.Clear();
        }

        // Cập nhật số lượng của sản phẩm đã chọn
        public void UpdateQuantity(int productId, int quantity)
        {
            var item = items.FirstOrDefault(i => i.ProductID == productId);
            if (item != null)
            {
                item.Quantity = quantity;
            }
        }
    }
}