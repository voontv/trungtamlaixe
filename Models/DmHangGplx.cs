using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class DmHangGplx
{
    /// <summary>
    /// Mã hạng GPLX
    /// </summary>
    public string MaHang { get; set; }

    /// <summary>
    /// Tên hạng GPLX
    /// </summary>
    public string TenHang { get; set; }

    /// <summary>
    /// Hạn sử dụng GPLX tính theo năm. 0=Vô thời hạn
    /// </summary>
    public int HanSuDung { get; set; }

    /// <summary>
    /// Nội dung ghi ở mặt sau GPLX bằng tiếng Việt
    /// </summary>
    public string MoTaVn { get; set; }

    /// <summary>
    /// Nội dung ghi ở mặt sau của GPLX bằng tiếng Anh
    /// </summary>
    public string MoTaEn { get; set; }

    public string GhiChu { get; set; }

    /// <summary>
    /// 1 = Hiệu lực; 2 = Không hiệu lực.
    /// </summary>
    public bool? TrangThai { get; set; }

    public string NguoiTao { get; set; }

    public string NguoiSua { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime NgaySua { get; set; }

    public string HangDuocLai { get; set; }

    public string MaHangMoi { get; set; }

    public string TenHangMoi { get; set; }

    public int? LoaiHang { get; set; }

    public string MoTaVncu { get; set; }

    public string MoTaEncu { get; set; }

    public virtual ICollection<KhoaHoc> KhoaHocs { get; set; } = new List<KhoaHoc>();

    public virtual ICollection<NguoiLxHoSo> NguoiLxHoSos { get; set; } = new List<NguoiLxHoSo>();
}
