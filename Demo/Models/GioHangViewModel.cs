namespace Demo.Models
{
    public class GioHangViewModel
    {
        //thuộc tính lưu trữ thông tin danh sách giỏ hàng
        public IEnumerable<GioHang> DsGioHang { get; set; }
        //thuộc tính lưu trữ tổng giá tiền của giỏ hàng
        // public double Total { get; set; }
        public HoaDon HoaDon { get; set; }
    }
}
