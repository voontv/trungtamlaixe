using System;

namespace Ttlaixe.DTO.request
{
    public class KhoaHocCreateRequest
    {

        public string MaCsdt { get; set; }
        public string MaSoGtvt { get; set; }
        public string TenKh { get; set; }
        public string HangGplx { get; set; }
        public string HangDt { get; set; }

        public string SoQdKhaiGiang { get; set; }          // optional
        public DateTime? NgayQdKhaiGiang { get; set; }     // optional

        public DateTime? NgayKg { get; set; }
        public DateTime? NgayBg { get; set; }
        public string MucTieuDt { get; set; }

        public int? ThoiGianDt { get; set; }               // ví dụ: 60
        public string GhiChu { get; set; }                 // optional
    }

}
