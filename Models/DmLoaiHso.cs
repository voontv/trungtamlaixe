using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class DmLoaiHso
{
    public int MaLoaiHs { get; set; }

    public string TenLoaiHs { get; set; }

    /// <summary>
    /// Yêu cầu đào tạo và sát hạch lại
    /// </summary>
    public string YeuCauDtshlai { get; set; }

    public string Nhom { get; set; }

    /// <summary>
    /// Thời hạn trả kết quả. Tính bằng ngày làm việc.
    /// </summary>
    public int? ThoiHanTraKq { get; set; }

    /// <summary>
    /// Văn bản pháp luật quy định điều kiện cấp
    /// </summary>
    public string SoVbpl { get; set; }

    /// <summary>
    /// Điều kiện được cấp GPLX
    /// </summary>
    public string DieuKien { get; set; }

    public string MaHtcap { get; set; }

    public int? MaNoiDungSh { get; set; }

    public string GhiChu { get; set; }

    public bool? TrangThai { get; set; }

    public string NguoiTao { get; set; }

    public string NguoiSua { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime NgaySua { get; set; }

    public virtual ICollection<NguoiLxHoSo> NguoiLxHoSos { get; set; } = new List<NguoiLxHoSo>();
}
