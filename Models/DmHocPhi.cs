using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class DmHocPhi
{
    public string MaHangGplx { get; set; }

    public decimal HocPhi { get; set; }

    public bool? IsActive { get; set; }

    public DateTime NgayKhoiTao { get; set; }

    public DateTime? NgayChinhSuaCuoiCung { get; set; }

    public virtual ICollection<HoSoHocPhi> HoSoHocPhis { get; set; } = new List<HoSoHocPhi>();
}
