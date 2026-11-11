
using System.Linq;
using System.Net;
using System.Web.Mvc;
using _25dh110430_MyStore.Areas.Admin.Filters;
using _25dh110430_MyStore.Models;

namespace _25dh110430_MyStore.Areas.Admin.Controllers
{

    public class OrdersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Orders
        public ActionResult Index()
        {
            var orders = db.Orders
                .Include("Customer")
                .ToList()
                .OrderBy(o =>
                    o.OrderStatus == "Chờ xác nhận" ? 0 :
                    o.OrderStatus == "Đã hủy" ? 2 : 1)
                .ThenByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        // POST: Admin/Orders/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(int id, string orderStatus)
        {
            var order = db.Orders.FirstOrDefault(o => o.OrderID == id);

            if (order == null)
                return HttpNotFound();

            string[] validStatuses =
            {
                "Chờ xác nhận",
                "Đã xác nhận",
                "Đang giao hàng",
                "Đã giao hàng",
                "Đã hủy"
            };

            if (!validStatuses.Contains(orderStatus))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            order.OrderStatus = orderStatus;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
