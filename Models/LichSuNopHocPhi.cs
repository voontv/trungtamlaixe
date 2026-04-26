using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class LichSuNopHocPhi
{
    public int IdNopTien { get; set; }

    public string MaDk { get; set; }

    public decimal SoTienNop { get; set; }

    public DateTime NgayNop { get; set; }

    public string HinhThucThanhToan { get; set; }

    public string SoBienLai { get; set; }

    public string GhiChu { get; set; }

    public DateTime NgayKhoiTao { get; set; }

    public virtual HoSoHocPhi MaDkNavigation { get; set; }
}
