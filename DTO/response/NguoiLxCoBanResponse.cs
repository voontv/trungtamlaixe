using System;

namespace Ttlaixe.DTO.response
{
    public class NguoiLxCoBanResponse
    {
        public string MaDk { get; set; }
        public string MaKhoaHoc { get; set; }
        public string MaCsdt { get; set; }

        public string HoVaTen { get; set; }
        /// <summary>
        /// Họ và tên đệm của Người lái xe
        /// </summary>
        public string HoDemNlx { get; set; }

        /// <summary>
        /// Tên của người lái xe
        /// </summary>
        /// 
        public string MaQuocTich { get; set; }
        public string TenNlx { get; set; }
        public string SoCmt { get; set; }
        public string? NgaySinh { get; set; }
        public string GioiTinh { get; set; }

        public string NoiThuongTru { get; set; }
        public string NoiCuTru { get; set; }
        public DateTime NgayNhanHso { get; set; }

        public string ImageUrl { get; set; }   // thêm cái này

    }

    public class NguoiLxThiResponse : NguoiLxCoBanResponse
    {
        /// <summary>
        /// Tên khóa học
        /// </summary>
        public string TenKh { get; set; }
        
    }
}
