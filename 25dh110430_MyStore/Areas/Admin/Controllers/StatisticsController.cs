using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using _25dh110430_MyStore.Areas.Admin.Filters;
using _25dh110430_MyStore.Models;
using _25dh110430_MyStore.Models.ViewModel;

namespace _25dh110430_MyStore.Areas.Admin.Controllers
{

    public class StatisticsController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Statistics
        public ActionResult Index(int pageSize = 5)
        {
            var model = new ProductStatisticsVM();

            // Giới hạn mỗi lần tăng thêm 5 sản phẩm
            pageSize = Math.Max(5, pageSize);
            model.PageSize = pageSize;

            model.TotalProducts = db.Products.Count();

            model.TotalStock = db.Products
                .Select(p => (int?)p.StockQuantity)
                .Sum() ?? 0;

            model.LowStockProducts = db.Products
                .Count(p => p.StockQuantity > 0 &&
                            p.StockQuantity <= 5);

            model.OutOfStockProducts = db.Products
                .Count(p => p.StockQuantity == 0);

            // Lấy thêm 1 sản phẩm để kiểm tra còn dữ liệu hay không
            var products = db.Products
                .Include(p => p.Category)
                .OrderBy(p => p.ProductID)
                .Take(pageSize + 1)
                .ToList();

            model.HasMore = products.Count > pageSize;

            model.Products = products
                .Take(pageSize)
                .ToList();

            // Thống kê 5 sản phẩm bán chạy nhất
            var bestSellingProducts = db.OrderDetails
                .GroupBy(od => new
                {
                    od.ProductID,
                    od.Product.ProductName
                })
                .Select(g => new
                {
                    ProductName = g.Key.ProductName,
                    TotalSold = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(5)
                .ToList();

            // Đưa dữ liệu vào ViewModel
            model.BestSellingProductNames = bestSellingProducts
                .Select(x => x.ProductName)
                .ToList();

            model.BestSellingQuantities = bestSellingProducts
                .Select(x => x.TotalSold)
                .ToList();

            return View(model);
        }

        // POST: Admin/Statistics/RestockProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RestockProduct(int id, int quantity)
        {
            if (quantity <= 0)
            {
                TempData["Error"] = "Số lượng nhập phải lớn hơn 0.";
                return RedirectToAction("Index");
            }

            var product = db.Products.Find(id);

            if (product == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm.";
                return RedirectToAction("Index");
            }

            product.StockQuantity += quantity;

            db.SaveChanges();

            TempData["Success"] = "Đã nhập thêm " + quantity +
                                  " sản phẩm " + product.ProductName +
                                  " thành công.";

            return RedirectToAction("Index");
        }
    }
}