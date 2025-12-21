using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class KhoaHoc
{
    /// <summary>
    /// Mã khóa học = &lt;MaSoGTVT&gt;&lt;MaCSDT&gt;-&lt;&apos;K-YY-(01-99)&gt;
    /// </summary>
    public string MaKh { get; set; }

    /// <summary>
    /// Mã Cơ sở đào tạo
    /// </summary>
    public string MaCsdt { get; set; }

    /// <summary>
    /// Mã Sở GTVT
    /// </summary>
    public string MaSoGtvt { get; set; }

    /// <summary>
    /// Tên khóa học
    /// </summary>
    public string TenKh { get; set; }

    public string HangGplx { get; set; }

    /// <summary>
    /// Hạng đào tạo lái xe. Lưu theo định dạng: A1|A2|A3|...
    /// </summary>
    public string HangDt { get; set; }

    /// <summary>
    /// Số Quyết định khai giảng khóa học
    /// </summary>
    public string SoQdKhaiGiang { get; set; }

    /// <summary>
    /// Ngày ra Quyết định khai giảng khóa học
    /// </summary>
    public DateTime? NgayQdKhaiGiang { get; set; }

    public DateTime? NgayKg { get; set; }

    public DateTime? NgayBg { get; set; }

    public string MucTieuDt { get; set; }

    /// <summary>
    /// Ngày thi
    /// </summary>
    public DateTime? NgayThi { get; set; }

    public DateTime? NgaySh { get; set; }

    /// <summary>
    /// Tổng số học viên của khóa học
    /// </summary>
    public int? TongSoHv { get; set; }

    public int? SoHvtotNghiep { get; set; }

    public int? SoHvduocCapGplx { get; set; }

    /// <summary>
    /// Thời gian đào tạo, tính theo tháng.
    /// </summary>
    public int? ThoiGianDt { get; set; }

    /// <summary>
    /// Số ngày ôn và kiểm tra kết thúc khóa học
    /// </summary>
    public int? SoNgayOnKt { get; set; }

    /// <summary>
    /// Số ngày thực học
    /// </summary>
    public int? SoNgayThucHoc { get; set; }

    /// <summary>
    /// Số ngày nghỉ lễ, khai, bế giảng
    /// </summary>
    public int? SoNgayNghiLe { get; set; }

    /// <summary>
    /// Cộng số ngày/Khóa đào tạo
    /// </summary>
    public int? TongSoNgay { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string GhiChu { get; set; }

    /// <summary>
    /// 0 = khong hieu luc; 1 = co hieu luc; mac dinh la 1;
    /// </summary>
    public bool? TrangThai { get; set; }

    public string NguoiTao { get; set; }

    public string NguoiSua { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime NgaySua { get; set; }

    public int? TtXuly { get; set; }

    public virtual DmHangDt HangDtNavigation { get; set; }

    public virtual DmHangGplx HangGplxNavigation { get; set; }

    public virtual ICollection<LichHoc> LichHocs { get; set; } = new List<LichHoc>();

    public virtual ICollection<NguoiLxHoSo> NguoiLxHoSos { get; set; } = new List<NguoiLxHoSo>();
}
