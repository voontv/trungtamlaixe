using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class NguoiLxhsGiayTo
{
    public int MaGt { get; set; }

    public string MaDk { get; set; }

    public string SoHoSo { get; set; }

    public string TenGt { get; set; }

    public bool? TrangThai { get; set; }

    public virtual NguoiLxHoSo MaDkNavigation { get; set; }

    public virtual DmGiayTo MaGtNavigation { get; set; }
}
