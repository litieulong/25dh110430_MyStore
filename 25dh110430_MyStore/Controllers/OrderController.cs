using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _25dh110430_MyStore.Models; // Namespace chứa Model của bạn
using _25dh110430_MyStore.Models.ViewModel; // Namespace chứa ViewModel của bạn

namespace _25dh110430_MyStore.Controllers
{
    public class OrderController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities(); // Khởi tạo kết nối database từ EDMX

        // GET: Order/Checkout
        public ActionResult Checkout()
        {
            // Lấy giỏ hàng từ Session để hiển thị tổng tiền và sản phẩm ra trang thanh toán
            // ...
            return View();
        }

        // POST: Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var order = new Order
            {
                CustomerID = model.CustomerID,
                OrderDate = DateTime.Now,
                TotalAmount = model.TotalAmount,
                PaymentStatus = "Chưa thanh toán",
                AddressDelivery = model.ShippingAddress,
                //DeliveryMethod = model.ShippingMethod,
                //PaymentMethod = model.PaymentMethod
            };

            db.Orders.Add(order);
            db.SaveChanges();

            return RedirectToAction("OrderSuccess", new { id = order.OrderID });
        }


        // GET: Order/OrderSuccess/{id}
        public ActionResult OrderSuccess(int id)
        {
            var order = db.Orders
                .Include("OrderDetails")
                .SingleOrDefault(o => o.OrderID == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }
    }
}