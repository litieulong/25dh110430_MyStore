using System.Collections.Generic;
using System.Reflection;
using _25dh110430_MyStore.Models;
using _25dh110430_MyStore.Models;
using PagedList.Mvc; // Thêm thư viện này vào đầu file

namespace _25dh110430_MyStore.Models.ViewModel
{
    public class ProductSearchVM
    {
        // tiêu chí để search theo tên, mô tả sp hoặc loại sản phẩm hoặc loại sản phẩm
        public string SearchTerm { get; set; }

        // Bổ sung hai thuộc tính khoảng giá để hết lỗi CS1061
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // Thứ tự sắp xếp
        public string SortOrder { get; set; }

        //Các thuộc tính hỗ trợ phân trang
        public int PageNumber { get; set; } // Trang hiện tại
        public int PageSize { get; set; } = 10; // Số sản phẩm trên mỗi trang (mặc định 10)

        // Kiểu dữ liệu phải là IPagedList<Product>
        public PagedList.IPagedList<Product> Products { get; set; }

        // danh sách sản phẩm thỏa điều kiện tìm kiếm sau khi phân trang
        //public List<Product> PagedProducts { get; set; }
    }
}