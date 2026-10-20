using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _25dh110430_MyStore.Models; // Namespace chứa Model của bạn

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
        public ActionResult Checkout(Order order)
        {
            // Xử lý lưu Order và OrderDetail vào database ở đây
            // ...
            return RedirectToAction("OrderSuccess");
        }

        // GET: Order/OrderSuccess
        public ActionResult OrderSuccess()
        {
            return View();
        }

        // GET: Order/OrderSuccess/{id}
        public ActionResult OrderSuccess(int id)
        {
            var order = db.Orders.Include("OrderDetails").SingleOrDefault(o => o.OrderID == id);
            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }
    }
}