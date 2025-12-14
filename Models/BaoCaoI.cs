using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class BaoCaoI
{
    /// <summary>
    /// Mã Báo cáo 1 = &lt;Mã Khóa học&gt;&lt;BCI&gt;
    /// </summary>
    public string MaBci { get; set; }

    /// <summary>
    /// Mã CSĐT. Ghi mã CSĐT trong bảng DM_DonViGTVT
    /// </summary>
    public string MaCsdt { get; set; }

    /// <summary>
    /// Mã Khóa học. Ghi mã Khóa học trong bảng dbo.KhoaHoc
    /// </summary>
    public string MaKh { get; set; }

    /// <summary>
    /// Số công văn của Báo cáo 1. Ví dụ: 084/LX-T31
    /// </summary>
    public string SoBaoCao { get; set; }

    /// <summary>
    /// Ngày gửi báo cáo 1
    /// </summary>
    public DateTime? NgayBaoCao { get; set; }

    /// <summary>
    /// Số Giấy phép đào tạo lái xe của CSĐT. Ví dụ: 51/CĐBVN-QLPT&amp;NL
    /// </summary>
    public string SoGp { get; set; }

    /// <summary>
    /// Ngày cấp giấy phép đào tạo lái xe của CSĐT
    /// </summary>
    public DateTime? NgayCapGp { get; set; }

    public int? LuuLuongGp { get; set; }

    public int? SoHocSinh { get; set; }

    public DateTime? NgayKg { get; set; }

    public DateTime? NgayBg { get; set; }

    /// <summary>
    /// Ngày tiếp nhận BC1
    /// </summary>
    public DateTime? NgayTiepNhan { get; set; }

    /// <summary>
    /// Người tiếp nhận BC1
    /// </summary>
    public string NguoiTiepNhan { get; set; }

    /// <summary>
    /// Kiểm tra thời gian tiếp nhận BC1 so với ngày khai giảng. Không quá 07 ngày đối với các hạng B1, B2, nâng hạng D, E; Không quá 15 ngày đối với hạng C
    /// </summary>
    public bool? ThoiGianTiepNhan { get; set; }

    /// <summary>
    /// Kiểm tra thời gian đào tạo  (Khai giảng - Bế giảng). 86 ngày hạng B1; 90 ngày hạng B2; 136 ngày hạng C; 30 ngày nâng hạng cấp 1; 52 ngày nâng hạng cấp 2.
    /// </summary>
    public bool? ThoiGianDaoTao { get; set; }

    /// <summary>
    /// Kiểm soát lưu lượng. Tại thời điểm khai giảng khóa đào tạo mới không vượt quá lưu lượng giấy phép.
    /// </summary>
    public bool? LuuLuong { get; set; }

    /// <summary>
    /// Kiểm tra bố trí học viên/xe tập lái tương ứng với 1 giáo viên giảng dạy thực hành/xe: 10 học viên/xe hạng B2; 16 học viên/xe hạng C; 10 học viên/xe nâng hạng 1 cấp; 20 học viên/xe nâng hạng 2 cấp.
    /// </summary>
    public bool? BoTriHocVienXeTap { get; set; }

    public string GhiChu { get; set; }

    /// <summary>
    /// Đã cập nhật kết quả = 1; Chưa cập nhật kết quả= 0; Giá trị mặc định = 0
    /// </summary>
    public bool TrangThai { get; set; }

    public string NguoiTao { get; set; }

    public string NguoiSua { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime NgaySua { get; set; }

    public int? SoHscanhBao { get; set; }

    public int? TtXuly { get; set; }

    public virtual ICollection<BaoCaoIi> BaoCaoIis { get; set; } = new List<BaoCaoIi>();
}
