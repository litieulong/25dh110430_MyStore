using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _25dh110430_MyStore.Models;

namespace _25dh110430_MyStore.Controllers
{
    public class CategoriesController : Controller
    {
        // Khởi tạo thực thể entities để truy vấn dữ liệu
        private MyStoreEntities db = new MyStoreEntities();
        // GET: Admin/Categories
        // GET: lấy dữ liệu từ bảng Category trong DB để hiển thị lên
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        // GET: Admin/Categories/Details/5
        // Details: lấy chi tiết một bản ghi có CategoryID = id
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                // mã lỗi 400: thiếu giá trị truyền vào
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null) // không tìm thấy bản ghi
            {
                return HttpNotFound(); // mã lỗi 404
            }
            return View(category);
        }
    }
}