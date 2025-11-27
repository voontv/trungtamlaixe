using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class DmLoaiHsoGiayTo
{
    public int MaGt { get; set; }

    public int MaLoaiHs { get; set; }

    public string MaHangGplx { get; set; }

    public string TenGt { get; set; }

    public string GhiChu { get; set; }

    public bool? TrangThai { get; set; }

    public string NguoiTao { get; set; }

    public string NguoiSua { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime NgaySua { get; set; }
}
