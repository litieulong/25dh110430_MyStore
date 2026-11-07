using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _25dh110430_MyStore.Models;

namespace _25dh110430_MyStore.Controllers
{
    public class ProductsController : Controller
    {

        private MyStoreEntities db = new MyStoreEntities();
        // GET: Products
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Travel()
        {
            int categoryId = 1003;

            var products = db.Products
                .Where(p => p.CategoryID == categoryId)
                .ToList();

            return View(products);
        }

        public ActionResult LapTop()
        {
            int categoryId = 1004;

            var products = db.Products
                .Where(p => p.CategoryID == categoryId)
                .ToList();

            return View(products);
        }

        public ActionResult Camera()
        {
            int categoryId = 1006;

            var products = db.Products
                .Where(p => p.CategoryID == categoryId)
                .ToList();

            return View(products);
        }

        public ActionResult Soldier()
        {
            int categoryId = 1005;

            var products = db.Products
                .Where(p => p.CategoryID == categoryId)
                .ToList();

            return View(products);
        }

        public ActionResult TravelBag()
        {
            return View();
        }

        public ActionResult CrossBodyBag()
        {
            return View();
        }

        public ActionResult LaptopBag()
        {
            return View();
        }

        public ActionResult CameraBag()
        {
            return View();
        }

        //public ActionResult TestStock()
        //{
        //    var product = db.Products.FirstOrDefault();

        //    if (product == null)
        //    {
        //        return Content("Không có sản phẩm");
        //    }

        //    return Content(
        //        "Tên sản phẩm: " + product.ProductName +
        //        " | Số lượng kho: " + product.StockQuantity
        //    );
        //}
    }
}