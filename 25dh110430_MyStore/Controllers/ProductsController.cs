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
            int categoryId = 1009;

            var products = db.Products
                .Where(p => p.CategoryID == categoryId)
                .ToList();

            return View(products);
        }

        public ActionResult CrossBodyBag()
        {
            int categoryId = 1010;

            var products = db.Products
                .Where(p => p.CategoryID == categoryId)
                .ToList();

            return View(products);
        }

        public ActionResult LaptopBag()
        {
            int categoryId = 1011;

            var products = db.Products
                .Where(p => p.CategoryID == categoryId)
                .ToList();

            return View(products);
        }

        public ActionResult CameraBag()
        {
            int categoryId = 1012;

            var products = db.Products
                .Where(p => p.CategoryID == categoryId)
                .ToList();

            return View(products);
        }
    }
}