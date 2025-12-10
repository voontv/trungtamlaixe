using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class DmQuocTich
{
    public string Ma { get; set; }

    public string TenEn { get; set; }

    public string TenVn { get; set; }

    public string GhiChu { get; set; }

    /// <summary>
    /// 0 = khong hieu luc; 1 = co hieu luc; mac dinh la 1;
    /// </summary>
    public bool? TrangThai { get; set; }

    public string NguoiTao { get; set; }

    public string NguoiSua { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime NgaySua { get; set; }

    public virtual ICollection<NguoiLx> NguoiLxes { get; set; } = new List<NguoiLx>();
}
