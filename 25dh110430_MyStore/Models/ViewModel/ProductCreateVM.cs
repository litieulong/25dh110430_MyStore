using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _25dh110430_MyStore.Models.ViewModel
{
    public class ProductColorVM
    {
        [Required(ErrorMessage = "Vui lòng nhập tên màu")]
        [Display(Name = "Tên màu")]
        public string ColorName { get; set; }

        public List<string> Images { get; set; }

        public ProductColorVM()
        {
            Images = new List<string>();
        }
    }

    public class ProductCreateVM
    {
        public int ProductID { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn danh mục hợp lệ")]
        [Display(Name = "Danh mục")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm không được vượt quá 100 ký tự")]
        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá sản phẩm")]
        [Range(typeof(decimal), "1", "999999999",
            ErrorMessage = "Giá sản phẩm phải lớn hơn 0")]
        [Display(Name = "Giá sản phẩm")]
        public decimal ProductPrice { get; set; }

        
        [Display(Name = "Mô tả sản phẩm")]
        public string ProductDescription { get; set; }

        [Range(0, int.MaxValue,
            ErrorMessage = "Số lượng tồn kho không được nhỏ hơn 0")]
        [Display(Name = "Số lượng tồn kho")]
        public int StockQuantity { get; set; }

        public List<ProductColorVM> Colors { get; set; }

        public ProductCreateVM()
        {
            Colors = new List<ProductColorVM>();
        }
    }
}