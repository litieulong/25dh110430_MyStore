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
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();

                users = users.Where(u =>
                    u.Username.Contains(searchTerm));
            }

            // Lọc theo vai trò
            if (!string.IsNullOrWhiteSpace(role))
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
    }
}