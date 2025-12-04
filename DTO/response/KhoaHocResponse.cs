using System;

namespace Ttlaixe.DTO.response
{
    public class KhoaHocResponse
    {
        /// <summary>
        /// Mã khóa học = &lt;MaSoGTVT&gt;&lt;MaCSDT&gt;-&lt;&apos;K-YY-(01-99)&gt;
        /// </summary>
        public string MaKh { get; set; }

        /// <summary>
        /// Mã Cơ sở đào tạo
        /// </summary>
        public string MaCsdt { get; set; }

        /// <summary>
        /// Mã Sở GTVT
        /// </summary>
        public string MaSoGtvt { get; set; }

        /// <summary>
        /// Tên khóa học
        /// </summary>
        public string TenKh { get; set; }

        public string HangGplx { get; set; }

        /// <summary>
        /// Hạng đào tạo lái xe. Lưu theo định dạng: A1|A2|A3|...
        /// </summary>
        public string HangDt { get; set; }

        public DateTime? NgayKg { get; set; }

        public DateTime? NgayBg { get; set; }

        /// <summary>
        /// Ngày thi
        /// </summary>
        public DateTime? NgayThi { get; set; }

        public DateTime? NgaySh { get; set; }

        /// <summary>
        /// Tổng số học viên của khóa học
        /// </summary>
        public int? TongSoHv { get; set; }

        public int? SoHvtotNghiep { get; set; }

        public int? SoHvduocCapGplx { get; set; }
    }
}
