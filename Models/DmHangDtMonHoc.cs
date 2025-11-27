using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class DmHangDtMonHoc
{
    public int MaMh { get; set; }

    public string MaHangDt { get; set; }

    public string TenMh { get; set; }

    public int? TongSoGio { get; set; }

    public int? LyThuyet { get; set; }

    public int? ThucHanhHinh { get; set; }

    public int? ThucHanhDuong { get; set; }

    public int? KiemTra { get; set; }

    public string GhiChu { get; set; }

    public bool? TrangThai { get; set; }

    public string NguoiTao { get; set; }

    public string NguoiSua { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime NgaySua { get; set; }

    public virtual DmHangDt MaHangDtNavigation { get; set; }
}
