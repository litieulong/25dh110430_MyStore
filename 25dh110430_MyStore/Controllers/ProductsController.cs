
using System;
using System.Linq;
using System.Web.Mvc;
using _25dh110430_MyStore.Models;

namespace _25dh110430_MyStore.Controllers
{
    public class ProductsController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // Hàm dùng chung cho tìm kiếm và sắp xếp
        private ActionResult GetProductsByCategory(
            int categoryId,
            string viewName,
            string categoryName,
            string searchTerm,
            string sortOrder)
        {
            var products = db.Products
                .Where(p => p.CategoryID == categoryId);

            // Tìm kiếm
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                products = products.Where(p =>
                    p.ProductName.Contains(searchTerm) ||
                    p.ProductDescription.Contains(searchTerm));
            }

            // Sắp xếp
            switch (sortOrder)
            {
                case "price_asc":
                    products = products.OrderBy(p => p.ProductPrice);
                    break;

                case "price_desc":
                    products = products.OrderByDescending(p => p.ProductPrice);
                    break;

                case "name_asc":
                    products = products.OrderBy(p => p.ProductName);
                    break;

                default:
                    products = products.OrderByDescending(p => p.ProductID);
                    break;
            }

            ViewBag.CategoryName = categoryName;

            return View(viewName, products.ToList());
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Travel(string searchTerm, string sortOrder)
        {
            return GetProductsByCategory(
                1003, "Travel", "Du lịch", searchTerm, sortOrder);
        }

        public ActionResult LapTop(string searchTerm, string sortOrder)
        {
            return GetProductsByCategory(
                1004, "LapTop", "Laptop", searchTerm, sortOrder);
        }

        public ActionResult Camera(string searchTerm, string sortOrder)
        {
            return GetProductsByCategory(
                1006, "Camera", "Máy ảnh", searchTerm, sortOrder);
        }

        public ActionResult Soldier(string searchTerm, string sortOrder)
        {
            return GetProductsByCategory(
                1005, "Soldier", "Quân đội", searchTerm, sortOrder);
        }

        public ActionResult TravelBag(string searchTerm, string sortOrder)
        {
            return GetProductsByCategory(
                1009, "TravelBag", "Balo du lịch", searchTerm, sortOrder);
        }

        public ActionResult CrossBodyBag(string searchTerm, string sortOrder)
        {
            return GetProductsByCategory(
                1010, "CrossBodyBag", "Túi đeo chéo", searchTerm, sortOrder);
        }

        public ActionResult LaptopBag(string searchTerm, string sortOrder)
        {
            return GetProductsByCategory(
                1011, "LaptopBag", "Balo đựng laptop", searchTerm, sortOrder);
        }

        public ActionResult CameraBag(string searchTerm, string sortOrder)
        {
            return GetProductsByCategory(
                1012, "CameraBag", "Túi máy ảnh", searchTerm, sortOrder);
        }

        public ActionResult Items(string searchTerm, string sortOrder)
        {
            return GetProductsByCategory(
                1013, "Items", "Phụ Kiện", searchTerm, sortOrder);
        }
    }
}
