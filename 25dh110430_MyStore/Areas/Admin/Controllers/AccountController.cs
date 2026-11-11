using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using _25dh110430_MyStore.Models;
using _25dh110430_MyStore.Areas.Admin.Filters;

namespace _25dh110430_MyStore.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Admin/Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            var account = db.Users.FirstOrDefault(u =>
                u.Username == username &&
                u.Password == password &&
                u.UserRole == "A");

            if (account == null)
            {
                ViewBag.Error = "Tài khoản hoặc mật khẩu không đúng!";
                return View();
            }

            Session["Username"] = account.Username;
            Session["UserRole"] = account.UserRole;

            FormsAuthentication.SetAuthCookie(account.Username, false);

            return RedirectToAction("Index", "Statistics");
        }

        // Đăng xuất
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();

            return RedirectToAction("Login");
        }

        // GET: Admin/Account/Register
        [AdminAuthorize]
        public ActionResult Register()
        {
            return View();
        }

        // POST: Admin/Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorize]
        public ActionResult Register(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            if (db.Users.Any(u => u.Username == username))
            {
                ViewBag.Error = "Username đã tồn tại.";
                return View();
            }

            var newAdmin = new User
            {
                Username = username,
                Password = password,
                UserRole = "A"
            };

            db.Users.Add(newAdmin);
            db.SaveChanges();

            ViewBag.Success = "Tạo tài khoản Admin thành công!";
            return View();
        }
    }
}