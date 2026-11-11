using _25dh110430_MyStore.Models;

namespace _25dh110430_MyStore.Models.ViewModel
{
    public class ProductSearchVM
    {
        // Tìm kiếm theo tên, mô tả hoặc loại sản phẩm
        public string SearchTerm { get; set; }

        // Lọc theo khoảng giá
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // Thứ tự sắp xếp
        public string SortOrder { get; set; }

        // Phân trang
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 10;

        // Danh sách sản phẩm sau khi tìm kiếm và phân trang
        public PagedList.IPagedList<Product> Products { get; set; }
    }
}