
using System;
using System.Collections.Generic;
using System.Linq;

namespace _25dh110430_MyStore.Models.ViewModel
{
    public class Cart
    {
        private List<CartItem> items = new List<CartItem>();

        public IEnumerable<CartItem> Items => items;

        public List<IGrouping<string, CartItem>> GroupedItems =>
            items.GroupBy(i => i.Category).ToList();

        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 6;

        public PagedList.IPagedList<Product> SimilarProducts { get; set; }

        // Thêm sản phẩm vào giỏ hàng
        public void AddItem(
            int productId,
            string productImg,
            string productName,
            decimal unitPrice,
            int quantity,
            string category,
            int? productColorId = null,
            string colorName = null)
        {
            // Tìm sản phẩm cùng mã và cùng màu
            var existingItem = items.FirstOrDefault(i =>
                i.ProductID == productId &&
                i.ProductColorID == productColorId);

            if (existingItem == null)
            {
                items.Add(new CartItem
                {
                    ProductID = productId,
                    ProductImage = productImg,
                    ProductName = productName,
                    UnitPrice = unitPrice,
                    Quantity = quantity,
                    Category = category,

                    // Lưu thông tin màu
                    ProductColorID = productColorId,
                    ColorName = colorName
                });
            }
            else
            {
                // Nếu cùng sản phẩm và cùng màu thì tăng số lượng
                existingItem.Quantity += quantity;
            }
        }

        // Xóa sản phẩm theo mã sản phẩm và mã màu
        public void RemoveItem(int productId, int? productColorId = null)
        {
            items.RemoveAll(i =>
                i.ProductID == productId &&
                i.ProductColorID == productColorId);
        }

        // Tính tổng tiền
        public decimal TotalValue()
        {
            return items.Sum(i => i.TotalPrice);
        }

        // Làm trống giỏ hàng
        public void Clear()
        {
            items.Clear();
        }

        // Cập nhật số lượng theo sản phẩm và màu
        public void UpdateQuantity(
            int productId,
            int quantity,
            int? productColorId = null)
        {
            var item = items.FirstOrDefault(i =>
                i.ProductID == productId &&
                i.ProductColorID == productColorId);

            if (item != null)
            {
                item.Quantity = quantity;
            }
        }
    }
}
