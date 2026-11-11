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
            // Lấy giỏ hàng từ Session
            var cartService = new CartService(Session);
            var cart = cartService.GetCart();

            if (cart == null || cart.Items == null || !cart.Items.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            // Kiểm tra đăng nhập
            var username = User.Identity.Name;

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            // Tìm khách hàng
            var customer = db.Customers
                .FirstOrDefault(c => c.Username == username);

            if (customer == null)
            {
                return Content("Không tìm thấy khách hàng: " + username);
            }

            // Tạo ViewModel
            var model = new CheckoutVM
            {
                CartItems = cart.Items.ToList(),
                CustomerID = customer.CustomerID,
                OrderDate = DateTime.Now,
                TotalAmount = cart.TotalValue(),
                PaymentStatus = "Chưa thanh toán",
                Username = username,
                ShippingAddress = customer.CustomerAddress
            };

            return View(model);
        }

        // POST: Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutVM model)
        {
            // KIỂM TRA ĐĂNG NHẬP
            var username = User.Identity.Name;

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            // LẤY GIỎ HÀNG TỪ SESSION
            var cartService = new CartService(Session);
            var cart = cartService.GetCart();

            if (cart == null || cart.Items == null || !cart.Items.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            // KIỂM TRA THÔNG TIN KHÁCH HÀNG
            var customer = db.Customers
                .FirstOrDefault(c => c.Username == username);

            if (customer == null)
            {
                return Content("Không tìm thấy thông tin khách hàng: " + username);
            }

            // KIỂM TRA ĐỊA CHỈ GIAO HÀNG
            if (string.IsNullOrWhiteSpace(model.ShippingAddress))
            {
                // Nếu form không gửi địa chỉ thì lấy địa chỉ đã lưu của khách hàng
                model.ShippingAddress = customer.CustomerAddress;
            }

            if (string.IsNullOrWhiteSpace(model.ShippingAddress))
            {
                ModelState.AddModelError(
                    "ShippingAddress",
                    "Vui lòng nhập địa chỉ giao hàng!"
                );
            }

            // KIỂM TRA THÔNG TIN FORM
            if (!ModelState.IsValid)
            {
                model.CartItems = cart.Items.ToList();
                model.TotalAmount = cart.TotalValue();

                return View(model);
            }

            try
            {
                // TẠO ĐƠN HÀNG
                var order = new Order
                {
                    CustomerID = customer.CustomerID,
                    OrderDate = DateTime.Now,

                    // Tính tiền từ giỏ hàng trên server
                    TotalAmount = cart.TotalValue(),

                    PaymentStatus = "Chưa thanh toán",
                    OrderStatus = "Chờ xác nhận",

                    // Lưu địa chỉ giao hàng
                    AddressDelivery = model.ShippingAddress.Trim(),


                };

                db.Orders.Add(order);

                // Lưu đơn hàng để lấy OrderID
                db.SaveChanges();

                // TẠO CHI TIẾT ĐƠN HÀNG
                var cartItems = cart.Items.ToList();

                if (cartItems.Count == 0)
                {
                    return Content("LỖI: Giỏ hàng không có sản phẩm.");
                }

                // KIỂM TRA TỒN KHO TRƯỚC KHI TRỪ
                foreach (var item in cartItems)
                {
                    var product = db.Products.Find(item.ProductID);

                    if (product == null)
                    {
                        return Content("Không tìm thấy sản phẩm ID: " + item.ProductID);
                    }

                    if (product.StockQuantity < item.Quantity)
                    {
                        return Content(
                            "Sản phẩm " + product.ProductName +
                            " không đủ hàng trong kho. Số lượng còn lại: " +
                            product.StockQuantity
                        );
                    }
                }

                // TẠO ORDER DETAIL VÀ TRỪ TỒN KHO
                foreach (var item in cartItems)
                {
                    var product = db.Products.Find(item.ProductID);

                    // Trừ số lượng tồn kho
                    product.StockQuantity -= item.Quantity;

                    // Tạo chi tiết đơn hàng
                    var orderDetail = new OrderDetail
                    {
                        OrderID = order.OrderID,
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };

                    db.OrderDetails.Add(orderDetail);
                }

                // Lưu OrderDetail và số lượng tồn kho mới
                db.SaveChanges();

                // Lưu OrderDetail
                int soDongDaLuu = db.SaveChanges();

                // Kiểm tra dữ liệu trong database qua EF
                int soDongTrongDB = db.OrderDetails
                    .Count(x => x.OrderID == order.OrderID);

                return RedirectToAction(
                      "OrderSuccess",
                      new { id = order.OrderID }
                );
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                string errorMessage = "";

                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        errorMessage +=
                            "Trường: " + validationError.PropertyName +
                            " - Lỗi: " + validationError.ErrorMessage +
                            "<br/>";
                    }
                }

                return Content(errorMessage);
            }
            catch (Exception ex)
            {
                return Content("Lỗi khi lưu đơn hàng: " + ex.Message);
            }
        }


        // GET: Order/OrderSuccess/{id}
        public ActionResult OrderSuccess(int id)
        {
            var order = db.Orders
                .Include("OrderDetails.Product")
                .FirstOrDefault(o => o.OrderID == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        // GET: Order/OrderHistory
        public ActionResult OrderHistory(string status)
        {
            var username = User.Identity.Name;

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Account");

            var customer = db.Customers
                .FirstOrDefault(c => c.Username == username);

            if (customer == null)
                return Content("Không tìm thấy thông tin khách hàng.");

            // Lấy đơn hàng của khách hàng hiện tại
            var orders = db.Orders
                .Where(o => o.CustomerID == customer.CustomerID);

            // Lọc theo trạng thái đơn hàng
            if (!string.IsNullOrWhiteSpace(status) && status != "TatCa")
            {
                orders = orders.Where(o => o.OrderStatus == status);
            }

            // Sắp xếp đơn hàng mới nhất lên đầu
            var result = orders
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            ViewBag.Status = status ?? "TatCa";

            return View(result);
        }

        // POST: Order/CancelOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelOrder(int id)
        {
            var username = User.Identity.Name;

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = db.Customers
                .FirstOrDefault(c => c.Username == username);

            if (customer == null)
            {
                return RedirectToAction("OrderHistory");
            }

            var order = db.Orders.FirstOrDefault(o =>
                o.OrderID == id &&
                o.CustomerID == customer.CustomerID
            );

            if (order == null)
            {
                return HttpNotFound();
            }

            // Chỉ cho phép hủy đơn đang chờ xác nhận
            if (order.OrderStatus == null ||
                order.OrderStatus.Trim() != "Chờ xác nhận")
            {
                return RedirectToAction("OrderHistory");
            }

            // Cập nhật trạng thái đơn hàng
            order.OrderStatus = "Đã hủy";
            order.CancelledAt = DateTime.Now;

            // LƯU THAY ĐỔI VÀO DATABASE
            db.SaveChanges();

            return RedirectToAction("OrderHistory");
        }
    }
}