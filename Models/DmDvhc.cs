using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class DmDvhc
{
    public string MaDvhc { get; set; }

    public string MaDvql { get; set; }

    public string MaDv { get; set; }

    public string TenDvhc { get; set; }

    public string TenNganGon { get; set; }

    public string TenDayDu { get; set; }

    public string LoaiDvhc { get; set; }

    public string GhiChu { get; set; }

    public bool? TrangThai { get; set; }

    public string NguoiTao { get; set; }

    public string NguoiSua { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime NgaySua { get; set; }

    public byte[] RowVersion { get; set; }
}
