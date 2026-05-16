using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class NhatKyChungTu
{
    public int IdChungTu { get; set; }

    public string SoChungTu { get; set; }

    public DateTime NgayLap { get; set; }

    public string DienGiai { get; set; }

    public string TaiKhoanNo { get; set; }

    public string TaiKhoanCo { get; set; }

    public decimal SoTien { get; set; }

    public string GhiChu { get; set; }

    public DateTime NgayKhoiTao { get; set; }

    public int? IdNopTien { get; set; }

    public virtual DmTaiKhoanKeToan TaiKhoanCoNavigation { get; set; }

    public virtual DmTaiKhoanKeToan TaiKhoanNoNavigation { get; set; }
}
