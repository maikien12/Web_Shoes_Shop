using Demo.Models;
using Demo.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Demo.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class GioHangController : Controller
    {
        private readonly ApplicationDbContext _db;
        public GioHangController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            //Lấy thông tin tài khoản
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);
            //Lấy danh sách sản phẩm trong giỏ hàng của user
            GioHangViewModel giohang = new GioHangViewModel()
            {
                DsGioHang = _db.GioHang
                .Include("SanPham")
                .Where(gh => gh.ApplicationUserId == claim.Value)
                .ToList(),
                HoaDon = new HoaDon()

            };
            foreach (var item in giohang.DsGioHang)
            {
                //Tính tiền sản phẩm theo SL
                item.ProductPrice = item.Quantity * item.SanPham.Price;
                //Cộng dồn tổng sl theo giỏ hàng
                giohang.HoaDon.Total += item.ProductPrice;
            }      
            return View(giohang);
        }
        //Action Tang
        public IActionResult Tang(int giohangId)
        {
            var giohang = _db.GioHang.FirstOrDefault(gh => gh.Id == giohangId);
            giohang.Quantity += 1;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        //Action Giam
        public IActionResult Giam(int giohangId)
        {
            //Lấy thông tin tương ứng với giỏ hàng id
            var giohang = _db.GioHang.FirstOrDefault(gh => gh.Id == giohangId);
            //Giảm số lượng sản phẩm đi 1
            giohang.Quantity -= 1; 
            //Nếu SL = 0 Thì xóa giỏ hàng
            if(giohang.Quantity == 0)
            {
                _db.GioHang.Remove(giohang);
            }    
            //Lưu CSDL
            _db.SaveChanges();
            //Quay về trang giỏ hàng
            return RedirectToAction("Index");
        }

        //Action Xoa
        public IActionResult Xoa(int giohangId)
        {
            // Lấy thông tin tương ứng với giỏ hàng Id
            var giohang = _db.GioHang.FirstOrDefault(gh => gh.Id == giohangId);
            //Xóa
            _db.GioHang.Remove(giohang);
            //Quay về CSDL
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult ThanhToan()
        {
            //Lấy thông tin tài khoản
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);
            //Lấy danh sách sản phẩm trong giỏ hàng của user
            GioHangViewModel giohang = new GioHangViewModel()
            {
                DsGioHang = _db.GioHang
                .Include("SanPham")
                .Where(gh => gh.ApplicationUserId == claim.Value)
                .ToList(),
                HoaDon = new HoaDon()

            };
            //Tìm Thông tin tài khoản trong CSDL
            giohang.HoaDon.ApplicationUser = _db.ApplicationUser.FirstOrDefault(user => user.Id == claim.Value);
            //Gán Thông tin tài khoản vào hóa đơn
            giohang.HoaDon.Name = giohang.HoaDon.ApplicationUser.Name;
            giohang.HoaDon.Address = giohang.HoaDon.ApplicationUser.Address;
            giohang.HoaDon.PhoneNumber = giohang.HoaDon.ApplicationUser.PhoneNumber;
            foreach (var item in giohang.DsGioHang)
            {
                //Tính tiền sản phẩm theo SL
                item.ProductPrice = item.Quantity * item.SanPham.Price;
                //Cộng dồn tổng sl theo giỏ hàng
                giohang.HoaDon.Total += item.ProductPrice;
            }
            return View(giohang);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThanhToan(GioHangViewModel giohang)
        {
            //Lấy thông tin tài khoản
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);
            //Cập nhật thông tin danh sách giỏ hàng và hóa đơn
            giohang.DsGioHang = _db.GioHang.Include("SanPham")
                .Where(gh => gh.ApplicationUserId == claim.Value)
                .ToList();
            giohang.HoaDon.ApplicationUserId = claim.Value;
            giohang.HoaDon.orderDate = DateTime.Now;
            giohang.HoaDon.orderStatus = "Đang Xác Nhận";
            foreach (var item in giohang.DsGioHang)
            {
                //Tính tiền sản phẩm theo SL
                item.ProductPrice = item.Quantity * item.SanPham.Price;
                //Cộng dồn tổng sl theo giỏ hàng
                giohang.HoaDon.Total += item.ProductPrice;
            }
            _db.HoaDon.Add(giohang.HoaDon);
            _db.SaveChanges();
            //Thêm Thông tin chi tiết hóa đơn
            foreach(var item in giohang.DsGioHang)
            {
                ChiTietHoaDon chitiethoadon = new ChiTietHoaDon()
                {
                    SanPhamId = item.SanPhamId,
                    HoaDonId = giohang.HoaDon.Id,
                    ProductPrice = item.ProductPrice,
                    Quantity = item.Quantity,
                    

                };
                _db.ChiTietHoaDon.Add(chitiethoadon);
                _db.SaveChanges();

            }
            //Xóa Thông tin Trong giỏ hàng
            _db.GioHang.RemoveRange(giohang.DsGioHang);
            _db.SaveChanges();
            return RedirectToAction("ConFirmation", "Home");
        }

    }
}
