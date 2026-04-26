using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class DmTaiKhoanKeToan
{
    public string MaTaiKhoan { get; set; }

    public string TenTaiKhoan { get; set; }

    public string MaTaiKhoanCha { get; set; }

    public string MaLoaiTaiKhoan { get; set; }

    public int Cap { get; set; }

    public int? SoThuTu { get; set; }

    public bool? IsActive { get; set; }

    public DateTime NgayKhoiTao { get; set; }

    public DateTime? NgayChinhSuaCuoiCung { get; set; }

    public virtual ICollection<NhatKyChungTu> NhatKyChungTuTaiKhoanCoNavigations { get; set; } = new List<NhatKyChungTu>();

    public virtual ICollection<NhatKyChungTu> NhatKyChungTuTaiKhoanNoNavigations { get; set; } = new List<NhatKyChungTu>();
}
