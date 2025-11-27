using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class DmHangDt
{
    /// <summary>
    /// Mã hạng đào tạo. Được ghi: A1; A2; ...; B1-B2; B2-C
    /// </summary>
    public string MaHangDt { get; set; }

    /// <summary>
    /// Tên hạng đào tạo
    /// </summary>
    public string TenHangDt { get; set; }

    /// <summary>
    /// Hạng GPLX
    /// </summary>
    public string HangGplx { get; set; }

    /// <summary>
    /// Số VBPL quy định
    /// </summary>
    public string SoVbpl { get; set; }

    /// <summary>
    /// Tuổi nhỏ nhất của học viên được phép đào tạo
    /// </summary>
    public int? TuoiHv { get; set; }

    /// <summary>
    /// Thâm niên nâng hạng lái xe
    /// </summary>
    public int? ThamNien { get; set; }

    /// <summary>
    /// Số Km lái xe an toàn
    /// </summary>
    public int? KmLxat { get; set; }

    /// <summary>
    /// Điều kiện sát hạch
    /// </summary>
    public string Dksh { get; set; }

    /// <summary>
    /// Mục tiêu đào tạo
    /// </summary>
    public string MucTieuDt { get; set; }

    public string GhiChu { get; set; }

    public bool? TrangThai { get; set; }

    public string NguoiTao { get; set; }

    public string NguoiSua { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime NgaySua { get; set; }

    public int? ThoiGianDaoTao { get; set; }

    public virtual ICollection<DmHangDtMonHoc> DmHangDtMonHocs { get; set; } = new List<DmHangDtMonHoc>();

    public virtual ICollection<KhoaHoc> KhoaHocs { get; set; } = new List<KhoaHoc>();
}
