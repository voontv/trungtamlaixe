using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class BaoCaoIi
{
    /// <summary>
    /// Mã Báo Cáo II = &lt;Mã CSĐT&gt;&lt;BCII&gt;&lt;YY&gt;&lt;01-99&gt;
    /// </summary>
    public string MaBcii { get; set; }

    public string MaBci { get; set; }

    public string MaCsdt { get; set; }

    /// <summary>
    /// Số công văn của Báo cáo II. Ví dụ: 123/BC-TTĐT&amp;TNCG
    /// </summary>
    public string SoBaoCao { get; set; }

    /// <summary>
    /// Ngày công văn của Báo cáo II
    /// </summary>
    public DateTime? NgayBaoCao { get; set; }

    /// <summary>
    /// Số thí sinh có trong Báo Cáo II
    /// </summary>
    public int? TongSoThiSinh { get; set; }

    public string GhiChu { get; set; }

    /// <summary>
    /// Đã cập nhật kết quả = 1; Chưa cập nhật kết quả= 0; Giá trị mặc định = 0
    /// </summary>
    public bool TrangThai { get; set; }

    public string NguoiTao { get; set; }

    public string NguoiSua { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime NgaySua { get; set; }

    public int? TtXuly { get; set; }

    public virtual BaoCaoI MaBciNavigation { get; set; }
}
