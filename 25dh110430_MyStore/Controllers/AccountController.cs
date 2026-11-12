using _25dh110430_MyStore.Models;
using _25dh110430_MyStore.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace _25dh110430_MyStore.Controllers
{
    public class AccountController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }


        public ActionResult Profile()
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
                return Content("Không tìm thấy thông tin khách hàng.");
            }

            return View(customer);
        }

        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kiểm tra tài khoản đã tồn tại
            var existingUser = db.Users
                .FirstOrDefault(u => u.Username == model.Username);

            if (existingUser != null)
            {
                ModelState.AddModelError(
                    "Username",
                    "Tên đăng nhập này đã tồn tại!"
                );

                return View(model);
            }

            // Tạo tài khoản
            var user = new User
            {
                Username = model.Username,
                Password = model.Password,
                UserRole = "C"
            };

            // Tạo thông tin khách hàng
            var customer = new Customer
            {
                Username = model.Username,
                CustomerName = model.CustomerName,
                CustomerEmail = model.CustomerEmail,
                CustomerPhone = model.CustomerPhone,
                CustomerAddress = model.CustomerAddress
            };

            try
            {
                // Lưu vào database
                db.Users.Add(user);
                db.Customers.Add(customer);

                db.SaveChanges();

                // Đăng nhập ngay sau khi đăng ký
                Session["Username"] = user.Username;
                Session["UserRole"] = user.UserRole;

                FormsAuthentication.SetAuthCookie(user.Username, false);

                // Chuyển về trang Home
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                db.Entry(user).State = System.Data.Entity.EntityState.Detached;
                db.Entry(customer).State = System.Data.Entity.EntityState.Detached;

                ModelState.AddModelError(
                    "",
                    "Đăng ký thất bại. Vui lòng kiểm tra lại thông tin."
                );

                return View(model);
            }
        }

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.SingleOrDefault(u =>
                    u.Username == model.Username &&
                    u.Password == model.Password
                );

                if (user != null)
                {
                    // Lưu trạng thái đăng nhập vào session
                    Session["Username"] = user.Username;
                    Session["UserRole"] = user.UserRole;

                    // Lưu thông tin xác thực người dùng vào cookie
                    FormsAuthentication.SetAuthCookie(user.Username, false);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }

            return View(model);
        }

        // GET: Account/Logout (Đăng xuất)
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();

            return RedirectToAction("Index", "Home");
        }
    }
}