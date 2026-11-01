using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using _25dh110430_MyStore.Models;
using _25dh110430_MyStore.Models.ViewModel;
using PagedList;

namespace _25dh110430_MyStore.Controllers
{
    public class HomeController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        public ActionResult Index2()
        {
            return View();
        }

        // GET: Admin/Products
        public ActionResult Index(string searchTerm, int? categoryId, int? page)
        {
            var model = new HomeProductVM();
            // lấy toàn bộ sản phẩm từ cơ 
            var products = db.Products.AsQueryable();
            // Tìm kiếm sản phẩm dựa trên từ khóa
            if (!string.IsNullOrEmpty(searchTerm))
            {
                model.SearchTerm = searchTerm;

                products = products.Where(p => p.ProductName.Contains(searchTerm) |
                                              p.ProductDescription.Contains(searchTerm) |
                                              p.Category.CategoryName.Contains(searchTerm));
            }

            // Đoạn code liên quan tới phân trang
            // Lấy số trang hiện tại (mặc định là trang 1 nếu không có giá trị)
            int pageNumber = page ?? 1;
            int pageSize = 6; // Số sản phẩm mỗi trang

            // Lấy top 10 sản phẩm bán chạy nhất
            model.FeaturedProducts = products.OrderByDescending(p => p.OrderDetails.Count()).Take(10).ToList();

            // Lấy 20 sản phẩm bán ế nhất và phân trang
            model.NewProducts = products.OrderBy(p => p.OrderDetails.Count()).Take(20).ToPagedList(pageNumber, pageSize);

            return View(model);
        }

        // GET: Home/ProductDetails/5
        public ActionResult ProductDetails(int? id, int? quantity, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product pro = db.Products
                .Include("ProductColors.ProductImages")
                .FirstOrDefault(p => p.ProductID == id);

            if (pro == null)
            {
                return HttpNotFound();
            }

            var products = db.Products
                .Where(p => p.CategoryID == pro.CategoryID
                         && p.ProductID != pro.ProductID)
                .AsQueryable();

            ProductDetailsVM model = new ProductDetailsVM();

            int pageNumber = page ?? 1;
            int pageSize = model.PageSize;

            model.product = pro;

            // Sản phẩm liên quan
            model.RelatedProducts = products
                .OrderBy(p => p.ProductID)
                .ToPagedList(pageNumber, pageSize);

            // Sản phẩm top trong cùng danh mục
            model.TopProducts = products
                .OrderByDescending(p => p.OrderDetails.Count())
                .Take(8)
                .ToPagedList(pageNumber, pageSize);

            // SẢN PHẨM NỔI BẬT - GIỐNG TRANG HOME
            model.TopProducts = db.Products
              .Where(p => p.ProductID != pro.ProductID)
              .OrderByDescending(p => p.OrderDetails.Count())
              .Take(8)
              .ToPagedList(pageNumber, pageSize);

            // Số lượng
            model.quantity = quantity ?? 1;

            // Tạm tính
            model.estimatedValue = pro.ProductPrice * model.quantity;

            return View(model);
        }
    }
}