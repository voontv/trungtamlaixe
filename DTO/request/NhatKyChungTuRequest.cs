using System;

namespace Ttlaixe.DTO.request
{
    public class NhatKyChungTuRequest
    {
        public string SoChungTu { get; set; }

        public DateTime NgayLap { get; set; }

        public string DienGiai { get; set; }

        public string TaiKhoanNo { get; set; }

        public string TaiKhoanCo { get; set; }

        public decimal SoTien { get; set; }

        public string GhiChu { get; set; }
    }
}
