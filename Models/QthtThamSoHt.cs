using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class QthtThamSoHt
{
    public string MaTs { get; set; }

    /// <summary>
    /// ALL: Tất cả; TW: Trung ương; TCDB: Tổng cục đường bộ; SOGTVT: Sở GTVT; TTSH: Trung tâm sát hạch; CSDT: Cơ sở đào tạo; VPDK: Văn phòng đăng ký
    /// </summary>
    public string DonViSuDung { get; set; }

    public string TenTs { get; set; }

    public string GiaTriTs { get; set; }

    public string GhiChu { get; set; }

    /// <summary>
    /// 0 = khong hieu luc; 1 = co hieu luc; mac dinh la 1;
    /// </summary>
    public bool? TrangThai { get; set; }

    public string NguoiTao { get; set; }

    public string NguoiSua { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime NgaySua { get; set; }

    public bool IsUpdate { get; set; }
}
