
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using System.Web.Mvc;
using _25dh110430_MyStore.Models;
using _25dh110430_MyStore.Areas.Admin.Filters;

namespace _25dh110430_MyStore.Areas.Admin.Controllers
{

    public class CustomersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Customers/Index
        public ActionResult Index(string searchTerm)
        {
            var customers = db.Customers
                .Include(c => c.User)
                .Where(c => c.User.UserRole == "C");

            // Tìm kiếm khách hàng
            if (!string.IsNullOrEmpty(searchTerm))
            {
                customers = customers.Where(c =>
                    c.CustomerName.Contains(searchTerm) ||
                    c.Username.Contains(searchTerm) ||
                    c.CustomerPhone.Contains(searchTerm) ||
                    c.CustomerEmail.Contains(searchTerm));
            }

            var customerList = customers
                .OrderBy(c => c.CustomerID)
                .ToList();

            // Tính tổng số lượng sản phẩm đã mua của từng khách hàng
            var purchasedQuantities = db.OrderDetails
                .GroupBy(od => od.Order.CustomerID)
                .Select(g => new
                {
                    CustomerID = g.Key,
                    TotalQuantity = g.Sum(od => od.Quantity)
                })
                .ToList()
                .ToDictionary(
                    x => x.CustomerID,
                    x => x.TotalQuantity
                );

            ViewBag.PurchasedQuantities = purchasedQuantities;
            ViewBag.SearchTerm = searchTerm;

            return View(customerList);
        }

        // GET: Admin/Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var customer = db.Customers
                .Include(c => c.User)
                .FirstOrDefault(c =>
                    c.CustomerID == id &&
                    c.User.UserRole == "C");

            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
