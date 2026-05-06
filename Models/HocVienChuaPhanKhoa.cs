using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class HocVienChuaPhanKhoa
{
    public int IdHs { get; set; }

    public string HoDemNlx { get; set; }

    public string TenNlx { get; set; }

    public string MaQuocTich { get; set; }

    public DateTime NgaySinh { get; set; }

    public string SoCmt { get; set; }

    public string HangDaoTao { get; set; }

    public string SoDienThoai { get; set; }

    public decimal SoTienNop { get; set; }

    public string MaGv { get; set; }

    public string GhiChu { get; set; }

    public DateTime NgayNopHoSo { get; set; }

    public string BangA1 { get; set; }

    public bool? CamKet { get; set; }

    public bool? AnhThe { get; set; }

    public bool? Don { get; set; }

    public bool? HopDong { get; set; }

    public bool? DonSatHach { get; set; }

    public bool? Gksk { get; set; }

    public bool? VanTayKhuonMat { get; set; }

    public bool? ChupAnh { get; set; }

    public string GioiTinh { get; set; }

    /// <summary>
    /// 0 = khong hieu luc; 1 = co hieu luc; mac dinh la 1;
    /// </summary>
    public bool? TrangThai { get; set; }

    public string MaDk { get; set; }

    public string DuongDanAnh { get; set; }
}
