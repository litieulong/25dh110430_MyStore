using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _25dh110430_MyStore.Models;
using _25dh110430_MyStore.Models.ViewModel;
using PagedList;

namespace _25dh110430_MyStore.Controllers
{
    public class CartController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();



        // Hàm lấy dịch vụ giỏ hàng
        private CartService GetCartService()
        {
            return new CartService(Session);
        }

        // Hiển thị giỏ hàng không gom nhóm theo danh mục
        // Hiển thị giỏ hàng
        public ActionResult Index()
        {
            var cart = GetCartService().GetCart();
            return View(cart); // Phải chắc chắn trả về đối tượng cart có kiểu Cart
        }

        // Thêm sản phẩm vào giỏ hàng
        public ActionResult AddToCart(int id, int quantity = 1)
        {
            var product = db.Products.Find(id);

            // Kiểm tra sản phẩm có tồn tại không
            if (product == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm!";
                return RedirectToAction("Index", "Home");
            }

            // Kiểm tra số lượng nhập
            if (quantity <= 0)
            {
                TempData["Error"] = "Số lượng sản phẩm không hợp lệ!";
                return RedirectToAction("ProductDetails", "Home",
                    new { id = id });
            }

            // Kiểm tra tồn kho
            if (product.StockQuantity <= 0)
            {
                TempData["Error"] = "Sản phẩm đã hết hàng!";
                return RedirectToAction("ProductDetails", "Home",
                    new { id = id });
            }

            // Không cho thêm vượt quá tồn kho
            if (quantity > product.StockQuantity)
            {
                TempData["Error"] = "Số lượng sản phẩm vượt quá tồn kho! Hiện chỉ còn "
                                    + product.StockQuantity + " sản phẩm.";
                return RedirectToAction("ProductDetails", "Home",
                    new { id = id });
            }

            // Thêm vào giỏ hàng
            var cartService = GetCartService();

            cartService.GetCart().AddItem(
                product.ProductID,
                product.ProductImage,
                product.ProductName,
                product.ProductPrice,
                quantity,
                product.Category.CategoryName
            );

            TempData["Success"] = "Đã thêm sản phẩm vào giỏ hàng!";

            return RedirectToAction("Index");
        }

        // Xóa sản phẩm khỏi giỏ
        public ActionResult RemoveFromCart(int id)
        {
            var cartService = GetCartService();
            cartService.GetCart().RemoveItem(id);
            return RedirectToAction("Index");
        }

        // Làm trống giỏ hàng
        public ActionResult ClearCart()
        {
            GetCartService().ClearCart();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            var cartService = GetCartService();
            cartService.GetCart().UpdateQuantity(id, quantity);
            return RedirectToAction("Index");
        }

        // Hiển thị giỏ hàng đã gom nhóm sản phẩm theo danh mục và hỗ trợ sản phẩm tương tự/phân trang
        public ActionResult Index2(int? page)
        {
            var cart = GetCartService().GetCart();
            var products = db.Products.ToList();
            var similarProducts = new List<Product>();

            if (cart.Items != null && cart.Items.Any())
            {
                similarProducts = products.Where(p => cart.Items.Any(ci => ci.Category == p.Category.CategoryName)
                    && !cart.Items.Any(ci => ci.ProductID == p.ProductID)).ToList();
            }

            // Đoạn code liên quan tới phân trang
            // Lấy số trang hiện tại (mặc định là trang 1 nếu không có giá trị)
            int pageNumber = page ?? 1;
            int pageSize = cart.PageSize;

            cart.SimilarProducts = similarProducts
                .OrderBy(p => p.ProductID)
                .ToPagedList(pageNumber, pageSize);

            return View(cart);
        }
    }
}
