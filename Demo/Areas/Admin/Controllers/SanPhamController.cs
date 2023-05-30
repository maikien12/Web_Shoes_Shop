using Demo.Data;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Demo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanPhamController : Controller
    {
        private readonly ApplicationDbContext _db;
        public SanPhamController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<SanPham> sanpham = _db.SanPham.Include("TheLoai").ToList();
            return View(sanpham);
        }
        //Action thêm mới và Cập nhật
        [HttpGet]
        public IActionResult Upsert(int id)
        {
            SanPham sanpham = new SanPham();
            IEnumerable<SelectListItem> dstheloai = _db.TheLoai.Select(theloai => new SelectListItem
            {
                Text = theloai.Name,
                Value =theloai.Id.ToString()

            });
            ViewBag.Dstheloai = dstheloai;
            if(id == 0)
            {
                //Tạo mới
                return View(sanpham);

            }  
            else
            {
                //Cập nhật
                sanpham = _db.SanPham.FirstOrDefault(sp => sp.Id == id);
                return View(sanpham);

            }
            
        }
        [HttpPost]
        public IActionResult Upsert(SanPham sanpham)
        {
            if(ModelState.IsValid)
            {
                if (sanpham.Id == 0)
                {
                    // Thêm mới
                    _db.Add(sanpham);
                }
                else
                {
                    _db.Update(sanpham);
                }    
                _db.SaveChanges();
                return RedirectToAction("Index");
            }    
            return View();
        }
        //Action Delete
        public IActionResult Delete(int id)
        {
            var sanpham = _db.SanPham.FirstOrDefault(sp => sp.Id == id);
            if(sanpham == null)
            {
                return NotFound();
            }    
            _db.SanPham.Remove(sanpham);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
