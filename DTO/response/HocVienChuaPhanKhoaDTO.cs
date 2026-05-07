using Ttlaixe.Models;

namespace Ttlaixe.DTO.response
{
    public class HocVienChuaPhanKhoaDTO
    {
        public HocVienChuaPhanKhoa HocVien {  get; set; }

        public string HoTenDem { get; set; }

        /// <summary>
        /// Tên của Giáo viên
        /// </summary>
        public string TenGv { get; set; }

        public string ImageUrl { get; set; }   // thêm cái này
    }
}
