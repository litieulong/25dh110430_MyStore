using System.Linq;
using System.Web.Mvc;
using _25dh110430_MyStore.Models;
using _25dh110430_MyStore.Areas.Admin.Filters;

namespace _25dh110430_MyStore.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class UsersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();


        // GET: Admin/Users
        public ActionResult Index(string searchTerm, string role)
        {
            var users = db.Users.AsQueryable();

            // Tìm kiếm theo Username
            if (!string.IsNullOrEmpty(searchTerm))
            {
                users = users.Where(u =>
                    u.Username.Contains(searchTerm));
            }

            // Lọc theo vai trò
            if (!string.IsNullOrEmpty(role))
            {
                users = users.Where(u => u.UserRole == role);
            }

            ViewBag.SearchTerm = searchTerm;
            ViewBag.Role = role;

            return View(users.OrderBy(u => u.Username).ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }

        // GET: Admin/Users/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            if (db.Users.Any(u => u.Username == user.Username))
            {
                ModelState.AddModelError(
                    "Username", "Username đã tồn tại!");
            }

            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(user);
        }

        //Phân quyền tài khoản
        // GET: Admin/Users/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeRole(string id, string role)
        {
            if (role != "A" && role != "C")
            {
                return new HttpStatusCodeResult(400);
            }

            var user = db.Users.FirstOrDefault(u => u.Username == id);

            if (user == null)
            {
                return HttpNotFound();
            }

            // Không cho Admin tự hạ quyền chính mình
            if (user.Username == User.Identity.Name && role == "C")
            {
                TempData["Message"] = "Không thể tự hạ quyền của mình!";
                return RedirectToAction("Index");
            }

            // Không cho hạ quyền Admin cuối cùng
            if (user.UserRole == "A" && role == "C")
            {
                int adminCount = db.Users.Count(u => u.UserRole == "A");

                if (adminCount <= 1)
                {
                    TempData["Message"] = "Phải có ít nhất một Admin!";
                    return RedirectToAction("Index");
                }
            }

            user.UserRole = role;
            db.SaveChanges();

            TempData["Message"] = "Cập nhật quyền thành công!";

            return RedirectToAction("Index");
        }
    }
}