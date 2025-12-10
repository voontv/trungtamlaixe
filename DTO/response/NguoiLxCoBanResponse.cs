using System;

namespace Ttlaixe.DTO.response
{
    public class NguoiLxCoBanResponse
    {
        public string MaDk { get; set; }
        public string MaKhoaHoc { get; set; }
        public string MaCsdt { get; set; }

        public string HoVaTen { get; set; }
        public string SoCmt { get; set; }
        public string? NgaySinh { get; set; }
        public string GioiTinh { get; set; }

        public string NoiThuongTru { get; set; }
        public string NoiCuTru { get; set; }
        public DateTime NgayNhanHso { get; set; }

    }

    public class NguoiLxThiResponse : NguoiLxCoBanResponse
    {
        /// <summary>
        /// Tên khóa học
        /// </summary>
        public string TenKh { get; set; }
    }
}
