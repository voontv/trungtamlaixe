using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class HoSoHocPhi
{
    public string MaDk { get; set; }

    public string MaKhoaHoc { get; set; }

    public string MaHangGplx { get; set; }

    public string HoVaTen { get; set; }

    public string NgaySinh { get; set; }

    public string SoCmt { get; set; }

    public string GioiTinh { get; set; }

    public string NoiCuTru { get; set; }

    public string NoiThuongTru { get; set; }

    public decimal HocPhi { get; set; }

    public bool? DaHoanThanhHp { get; set; }

    public bool? BoHoc { get; set; }

    public DateTime NgayKhoiTao { get; set; }

    public DateTime? NgayChinhSuaCuoiCung { get; set; }

    public virtual ICollection<LichSuNopHocPhi> LichSuNopHocPhis { get; set; } = new List<LichSuNopHocPhi>();

    public virtual DmHocPhi MaHangGplxNavigation { get; set; }
}
