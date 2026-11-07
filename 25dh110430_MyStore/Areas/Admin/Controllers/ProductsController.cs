using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using _25dh110430_MyStore.Models;
using _25dh110430_MyStore.Models.ViewModel;
using _25dh110430_MyStore.Areas.Admin.Filters;
using PagedList;


namespace _25dh110430_MyStore.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        

        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = db.Products
                .Include("ProductColors.ProductImages")
                .FirstOrDefault(p => p.ProductID == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            foreach (var color in product.ProductColors)
            {
                var images = db.ProductImages
                    .Where(x => x.ProductColorID == color.ProductColorID)
                    .ToList();

                System.Diagnostics.Debug.WriteLine(
                    "MÀU: " + color.ColorName +
                    " - SỐ ẢNH: " + images.Count
                );

                foreach (var image in images)
                {
                    System.Diagnostics.Debug.WriteLine(
                        "    IMAGE: " + image.ImagePath
                    );
                }
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID =
                new SelectList(
                    db.Categories,
                    "CategoryID",
                    "CategoryName"
                );

            var model = new ProductCreateVM();

            // Tạo sẵn 1 màu
            model.Colors.Add(new ProductColorVM());

            return View(model);
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> Create(ProductCreateVM model)
        {
           
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryID = new SelectList(
                    db.Categories,
                    "CategoryID",
                    "CategoryName",
                    model.CategoryID
                );

                return View(model);
            }

            // 1. LẤY LINK ẢNH ĐẦU TIÊN
            string firstImageUrl = null;

            if (model.Colors != null)
            {
                foreach (var color in model.Colors)
                {
                    if (color.Images != null)
                    {
                        foreach (var imageUrl in color.Images)
                        {
                            if (!string.IsNullOrWhiteSpace(imageUrl))
                            {
                                firstImageUrl = imageUrl;
                                break;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(firstImageUrl))
                        break;
                }
            }

            // 2. LƯU SẢN PHẨM
            Product product = new Product();

            product.CategoryID = model.CategoryID;
            product.ProductName = model.ProductName;
            product.ProductPrice = model.ProductPrice;
            product.ProductDescription = model.ProductDescription;
            product.StockQuantity = model.StockQuantity;

            // ProductImage là trường bắt buộc
            // Nếu có link ảnh thì lấy link ảnh đầu tiên
            // Nếu chưa nhập ảnh thì dùng ảnh mặc định
            product.ProductImage = firstImageUrl ?? "default.jpg";

            db.Products.Add(product);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                string error = "";

                foreach (var entityErrors in ex.EntityValidationErrors)
                {
                    error += "Entity: "
                           + entityErrors.Entry.Entity.GetType().Name
                           + "<br/>";

                    foreach (var validationError in entityErrors.ValidationErrors)
                    {
                        error += "Property: "
                               + validationError.PropertyName
                               + " - Error: "
                               + validationError.ErrorMessage
                               + "<br/>";
                    }
                }

                return Content(error);
            }

            // 3. LƯU MÀU SẮC VÀ LINK HÌNH ẢNH
            if (model.Colors != null)
            {
                foreach (var color in model.Colors)
                {
                    if (string.IsNullOrWhiteSpace(color.ColorName))
                        continue;

                    ProductColor productColor = new ProductColor();

                    productColor.ProductID = product.ProductID;
                    productColor.ColorName = color.ColorName;

                    db.ProductColors.Add(productColor);

                    await db.SaveChangesAsync();

                    // 4. LƯU CÁC LINK HÌNH ẢNH
                    if (color.Images != null)
                    {
                        foreach (var imageUrl in color.Images)
                        {
                            if (string.IsNullOrWhiteSpace(imageUrl))
                                continue;

                            ProductImage productImage = new ProductImage();

                            productImage.ProductColorID =
                                productColor.ProductColorID;

                            productImage.ImagePath = imageUrl;

                            db.ProductImages.Add(productImage);
                        }

                        await db.SaveChangesAsync();
                    }
                }
            }

            var savedColors = db.ProductColors
    .Where(x => x.ProductID == product.ProductID)
    .ToList();


            return RedirectToAction("Index");
        }

        // GET: Admin/Products/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = await db.Products.FindAsync(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            ViewBag.CategoryID = new SelectList(
                db.Categories,
                "CategoryID",
                "CategoryName",
                product.CategoryID
            );

            var model = new ProductCreateVM();

            model.ProductID = product.ProductID;
            model.CategoryID = product.CategoryID;
            model.ProductName = product.ProductName;
            model.ProductPrice = product.ProductPrice;
            model.ProductDescription = product.ProductDescription;
            model.StockQuantity = product.StockQuantity;

            return View(model);

           
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProductCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryID = new SelectList(
                    db.Categories,
                    "CategoryID",
                    "CategoryName",
                    model.CategoryID
                );

                return View(model);
            }

            // Tìm sản phẩm thật trong database
            Product product = await db.Products.FindAsync(model.ProductID);

            if (product == null)
            {
                return HttpNotFound();
            }

            // Cập nhật thông tin sản phẩm
            product.CategoryID = model.CategoryID;
            product.ProductName = model.ProductName;
            product.ProductPrice = model.ProductPrice;
            product.ProductDescription = model.ProductDescription;
            product.StockQuantity = model.StockQuantity;

            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: Admin/Products/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = await db.Products.FindAsync(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            var productColors = db.ProductColors
                .Where(x => x.ProductID == id)
                .ToList();

            foreach (var color in productColors)
            {
                var productImages = db.ProductImages
                    .Where(x => x.ProductColorID == color.ProductColorID)
                    .ToList();

                db.ProductImages.RemoveRange(productImages);
            }

            db.ProductColors.RemoveRange(productColors);

            db.Products.Remove(product);

            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult Index(
    string searchTerm,
    decimal? minPrice,
    decimal? maxPrice,
    string sortOrder,
    int? page)
        {
            var model = new ProductSearchVM();
            var products = db.Products.AsQueryable();

            // Tìm kiếm sản phẩm theo từ khóa
            if (!string.IsNullOrEmpty(searchTerm))
            {
                model.SearchTerm = searchTerm;

                products = products.Where(p =>
                    p.ProductName.Contains(searchTerm) ||
                    p.ProductPrice.ToString().Contains(searchTerm) ||
                    p.ProductDescription.Contains(searchTerm));
            }

            // Giá tối thiểu
            if (minPrice.HasValue)
            {
                model.MinPrice = minPrice.Value;

                products = products.Where(p =>
                    p.ProductPrice >= minPrice.Value);
            }

            // Giá tối đa
            if (maxPrice.HasValue)
            {
                model.MaxPrice = maxPrice.Value;

                products = products.Where(p =>
                    p.ProductPrice <= maxPrice.Value);
            }

            // Sắp xếp
            switch (sortOrder)
            {
                case "name_asc":
                    products = products.OrderBy(p => p.ProductName);
                    break;

                case "name_desc":
                    products = products.OrderByDescending(p => p.ProductName);
                    break;

                case "price_asc":
                    products = products.OrderBy(p => p.ProductPrice);
                    break;

                case "price_desc":
                    products = products.OrderByDescending(p => p.ProductPrice);
                    break;

                default:
                    products = products.OrderBy(p => p.ProductName);
                    break;
            }

            model.SortOrder = sortOrder;

        // Xử lý phân trang
        //lấy trạng hiện tại của trang từ query string, nếu không có thì mặc định là trang 1
        int pageNumber = page ?? 1; // Trang hiện tại, mặc định là trang 1
            int pageSize = 2; // Số lượng sản phẩm hiển thị trên 1 trang (có thể đổi thành 5, 10...)

            // Sử dụng ToPagedList để phân trang danh sách sản phẩm
            //model.Products = products.ToPagedList();
            model.Products = products.ToPagedList(pageNumber, pageSize);

            return View(model);
        }
    }
}