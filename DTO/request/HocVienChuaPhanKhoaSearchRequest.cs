using System;

namespace Ttlaixe.DTO.request
{
    public class HocVienChuaPhanKhoaSearchRequest
    {
        public string? HoVaTen { get; set; }
        public string? MaQuocTich { get; set; }
        public string? HangDaoTao { get; set; }
        public string? SoDienThoai { get; set; }
        public string? TenGiaoVien { get; set; }
        public string? MaGV { get; set; }

        public DateTime? NgaySinhFrom { get; set; }
        public DateTime? NgaySinhTo { get; set; }

        public DateTime? NgayNopHoSoFrom { get; set; }
        public DateTime? NgayNopHoSoTo { get; set; }

        public decimal? SoTienNopFrom { get; set; }
        public decimal? SoTienNopTo { get; set; }

        // filter giấy tờ
        public bool? CamKet { get; set; }
        public bool? AnhThe { get; set; }
        public bool? Don { get; set; }
        public bool? HopDong { get; set; }
        public bool? DonSatHach { get; set; }
        public bool? GKSK { get; set; }
        public bool? VanTayKhuonMat { get; set; }
        public bool? ChupAnh { get; set; }
    }
}
