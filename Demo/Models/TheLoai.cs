using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class TheLoai
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="Không Được để trống !")]
        public string Name { get; set; }
        [Required(ErrorMessage ="Không dùng định dạng này !")]
        [Display(Name ="Ngày tạo")]
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
